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

                DataSourceField consumption = new DataSourceField(
                        "consumption",
                        DataSourceField.TYPE_SCALAR);

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
                 * Note the new Model Type parameter, you dont pass the model type
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

                DataTable input = new DataTable(new FileInfo(filename));
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

                    Thread.Sleep(swarm.Expires * 1000);
                }

                // Success!
                Console.WriteLine("You win! Your Grok Swarm is complete.");

                DateTime EndTime = DateTime.Now;
                TimeSpan Duration = EndTime.Subtract(StartTime);
                Console.WriteLine("Total time was " + Duration.TotalSeconds + " seconds");

                // Retrieve Swarm results
                Console.WriteLine("Getting results from Swarm");
                DataTable output = HelloGrokModel.RetrieveData();

                // Write results out to a CSV
                DirectoryInfo OutputDirectory = new DirectoryInfo("output");
                // Create that directory if it doesn't exist
                if (!OutputDirectory.Exists)
                {
                    OutputDirectory.Create();
                }

                string OutputFile = "output/AnomalyScores.csv";
                Console.WriteLine("Saving results to " + OutputFile);
                output.WriteCSV(new FileInfo(OutputFile));

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
