// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;
using System.Configuration;
using System.IO;

using Grok.Numenta;
using System.Threading;
using System.Collections.Generic;
using CsvHelper;

namespace HelloGrok
{
    /// <summary>
    /// Welcome to part three of the Hello Grok Tutorial!
    /// 
    /// In this tutorial we will:
    /// 
    /// Create a new model optimized to detect anomalies, promote the model to production,
    /// Stream new records and detect which of these new records are anomalous.
    /// 
    /// </summary>
    public class HelloAnomaly
    {
        public static void RunHelloAnomaly()
        {
            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            // Set up our client variable for later use.
            APIClient client = null;

            try
            {
                Console.WriteLine("Starting Grok");

                // Create a client and connect to the Grok API
                client = new APIClient(
                        ConfigurationManager.AppSettings["APIKey"],
                        ConfigurationManager.AppSettings["ServerURL"]);

                Console.WriteLine("Successfully connected to API server.");

                // Create a new project
                Console.WriteLine("Creating and configuring a new project");
                string ProjectName = "Recreation Center Anomaly Test " + (new DateTime()).ToString();
                Project project = client.CreateProject(ProjectName);

                /*
                 * Configure the project
                 * 
                 * Grok needs to know how to handle the data that you give it. Data
                 * should be in CSV format. We'll need to tell Grok how to interpret
                 * each of the fields in that CSV.
                 */
                DataSourceField timestamp = new DataSourceField(
                        "timestamp",
                        DataSourceField.TYPE_DATETIME,
                        DataSourceField.FLAG_TIMESTAMP);

                // Make sure to specify the field's min and max values.
                DataSourceField consumption = new DataSourceField("consumption",0.0,100.0);

                DataSource dataSource = new DataSource();
                dataSource.name = "My Anomaly Data Source";
                dataSource.dataSourceType = "local";
                dataSource.fields.Add(timestamp);
                dataSource.fields.Add(consumption);

                Grok.Numenta.Stream stream = new Grok.Numenta.Stream();
                stream.name = "My Anomaly Stream";
                stream.dataSources.Add(dataSource);
                stream = project.CreateStream(stream);

                /*
                 * Aggregate our values into hour buckets
                 * 
                 * Aggregation can help smooth out choppy data and lets you change
                 * the time scale for which you are making predictions. Here we want
                 * to know energy use in the next hour, but our dataset has values
                 * at 15 minute intervals, so we specify that we want to SUM all the
                 * values that fall into a given hour, and use that sum as the input
                 * to our model.
                 */
                Console.WriteLine("Using aggregation for this Swarm");
                TimeAggregation aggregation = new TimeAggregation();
                aggregation.SetHours(1);
                aggregation.FieldOverrides.Add(new string[] { consumption.name, TimeAggregation.FUNCTION_MEAN });

                // Create a model within our project
                Console.WriteLine("Creating a new Anomaly model.");

                /*
                 * Note the new Model Type parameter, if you dont pass the model type
                 * Grok will use Model.TYPE_PREDICTOR as default.
                 * In our case we want to create an anmomaly model (Model.TYPE_ANOMALY).
                 * As a result, two new fields will be added to the output:
                 *  "Anomaly Score" and "Anomaly Label"
                 */
                Model HelloGrokModel = new Model(Model.TYPE_ANOMALY);
                HelloGrokModel.name = "Hello Grok Anomaly - Rec Center - " + DateTime.Now.ToString();
                HelloGrokModel.streamId = stream.id;
                HelloGrokModel.predictedField = consumption.name;
                HelloGrokModel.aggregation = aggregation;
                HelloGrokModel = project.CreateModel(HelloGrokModel);

                // Stream data from a local file source
                string filename = ConfigurationManager.AppSettings["filename"];
                Console.WriteLine("Streaming data from local file " + filename
                        + " to Model " + HelloGrokModel.id);

                DataTable input = new DataTable(new FileInfo(filename), false, 4000);
                Console.WriteLine("Starting data upload...");
                stream.AppendData(input, new SampleUploadCallback());
                Console.WriteLine("Data successfully streamed to Grok!");

                /*
                 * Start the Grok Swarm
                 * 
                 * Now that we have a carefully configured model, and that model has
                 * data to work with, we can now begin searching all the
                 * configuration variables that go into making a great predictive
                 * model. This is what we call a Grok Swarm.
                 */
                Console.WriteLine("Starting Swarm");
                Swarm swarm = HelloGrokModel.SwarmModel(Swarm.SIZE_MEDIUM);
                Console.WriteLine("Swarm ID = " + swarm.id);

                DateTime StartTime = DateTime.Now;

                /*
                 * Monitor the Swarm's progress
                 * 
                 * During each iteration we are looking at the error of the best
                 * configuration we've found so far.
                 * 
                 * Grok adapts to data as it encounters it, so at any given moment
                 * the error may go up or down. Over time however a Grok model will
                 * learn the patterns in your data and generate better and better
                 * predictions.
                 */
                while (true)
                {
                    swarm = swarm.Retrieve();
                    Console.WriteLine("Swarm status = " + swarm.status);

                    if ("running".Equals(swarm.status))
                    {
                        Console.WriteLine(swarm.details.ToString());
                    }

                    if ("completed".Equals(swarm.status) || "error".Equals(swarm.status))
                    {
                        break;
                    }
                    // Wait until this result expires before refreshing the swarm object.
                    // The server will use this property to hint when the client should check for an update.
                    Thread.Sleep(swarm.Expires * 1000);
                }

                // Success!
                Console.WriteLine("You win! Your Grok Swarm is complete.");

                Console.WriteLine("Promoting model");
                HelloGrokModel.Promote(true);

                /*
                 * Anomaly models use 0.8 as the anomaly score threshold for default anomaly classification.
                 * We can override this threshold using the "SetAnomalyThreshold" API call.
                 * In our case we are lowering the threshold to 0.7.
                 */
                HelloGrokModel.SetAnomalyThreshold(0.7);

                /*
                 * Anomaly models will skip the first records of the dataset before 
                 * it starts detecting and labeling anomalies. 
                 * It uses the swarm size as the default value for this parameter. 
                 * 
                 * We will override and tell the model to wait for the first 1000 records
                 * before labeling anomalies.
                 */
                HelloGrokModel.SetAutoDetectWaitRecords(1000);

                /*
                 * Add a little wonkiness to the data for September 1, 2010. We add +/- 5 to the
                 * energy reading and a small scale factor. This should show up as unpredicted,
                 * and therefore have higher than normal anomaly scores. Note that a threshold
                 * based system would not detect this behavior as an anomaly.
                */
                Console.WriteLine("Adding some artificial anomalies on September 1, 2010 ...");
                DataTable streamData = new DataTable(new FileInfo(filename), false, 4000, 4000);
                List<String[]> newRecords = streamData.Data;
                double[] offset = { -5, -5, -5, -5, 5, 5, 5, 5 };
                for (int i=1856;i<1952;i++) {
                    String [] row = newRecords[i];
                    row[1] = (Double.Parse(row[1])*0.75 + offset[i%8]).ToString();
                    newRecords[i] = row;
                }

                // Data in
                Console.WriteLine("Sending records...");
                stream.AppendData(streamData);
                Console.WriteLine("Data successfully streamed to Grok!");
            
                // Wait a few seconds...
                Console.WriteLine("Waiting for Grok to process...");
                HelloGrokModel.WaitForStabilization(true);

                // Retrieve Swarm results
                Console.WriteLine("Getting results from Swarm");
                DataTable results = HelloGrokModel.RetrieveData(2500);
                
                // Get anomaly field index from results
                Console.WriteLine("Retrieving anomalies ...");
                Dictionary<int, List<String>> labels = HelloGrokModel.GetLabels();
                int anomalyField = results.AnomalyScoreFieldIndex;
            
                // Update result with anomaly labels
                List<String[]> resultsRecords = results.Data;
                for (int i=0;i<resultsRecords.Count;i++) {
                    String [] row = resultsRecords[i];
                    List<String> rowLabels;
                    if (labels.TryGetValue(int.Parse(row[0]), out rowLabels)) {
                        row[anomalyField] = string.Join(",", rowLabels);
                        resultsRecords[i]=row;
                    }
                }

                // Write results out to a CSV
                DirectoryInfo OutputDirectory = new DirectoryInfo("output");
                // Create that directory if it doesn't exist
                if (!OutputDirectory.Exists)
                {
                    OutputDirectory.Create();
                }

                string OutputFile = "output/AnomalyScores.csv";
                Console.WriteLine("Saving results to " + OutputFile);
                results.WriteCSV(new FileInfo(OutputFile));

                // Fin
                Console.WriteLine("You can now inspect your results. \n\n");

                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                // Display encountered errors on console
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
