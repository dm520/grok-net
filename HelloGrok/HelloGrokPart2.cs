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

namespace HelloGrok
{
    public class HelloGrokPart2
    {
        public static void RunHelloGrokPart2()
        {
            // Set up our client variable for later use.
		    APIClient client = null;

		    // Put in the model Id from part one to get started.
		    String ModelId = ConfigurationManager.AppSettings["ModelID"];

            try
            {
                // Re-establish a connection to Grok
                Console.WriteLine("Starting Grok");

                // Create a client and connect to the Grok API
                client = new APIClient(
                        ConfigurationManager.AppSettings["APIKey"],
                        ConfigurationManager.AppSettings["ServerURL"]);

                Console.WriteLine("Successfully connected to API server.");

                // Promote our model to production ready status
                Console.WriteLine("Retrieving model");
                Model model = client.RetrieveModelById(ModelId);

                /******************************************************************************
                 * Promote the model
                 *
                 * Note: This will eventually go away in favor of automatic promotion
                 *
                 * When you promote a model you are taking it from the swarm into a
                 * production ready state. You can then train the model further by sending
                 * it more records (as we will do below) or you can start streaming your
                 * live data right away.
                 *
                 * The current implementation has a quirk where the system will first
                 * rerun the data from the swarm so that the production model is 'caught up.'
                 * This means that the first new predictions you get back from the production
                 * model will have ROWIDs greater than 0.
                 */

                Console.WriteLine("Promoting model");
                model.Promote(true);
                model.DisableLearning();

                /*
                 * Stream in a few records to our production model.
                 * 
                 * This is very inefficient, but if your data stream has infrequent
                 * updates this may be sufficient.
                 * 
                 * In this example we're streaming in un-aggregated data. Recall
                 * that our stream definition called for data to be aggregated over
                 * 1 Hour. The production model will wait until it has seen all the
                 * records for a given hour before making a prediction about the
                 * next.
                 */

                Grok.Numenta.Stream stream = client.RetrieveStreamById(model.streamId);

                string[][] recordsToStream = {
					    new string [] { "2011-01-01 00:00:00.0", "5.3" },
					    new string [] { "2011-01-01 00:15:00.0", "5.5" },
					    new string [] { "2011-01-01 00:30:00.0", "5.1" },
					    new string [] { "2011-01-01 00:45:00.0", "5.3" },
					    new string [] { "2011-01-01 01:00:00.0", "5.2" },
					    new string [] { "2011-01-01 01:15:00.0", "5.5" },
					    new string [] { "2011-01-01 01:30:00.0", "4.5" },
					    new string [] { "2011-01-01 01:45:00.0", "1.2" },
					    new string [] { "2011-01-01 02:00:00.0", "1.1" },
					    new string [] { "2011-01-01 02:15:00.0", "1.2" },
					    new string [] { "2011-01-01 02:30:00.0", "1.2" },
					    new string [] { "2011-01-01 02:45:00.0", "1.2" },
					    new string [] { "2011-01-01 03:00:00.0", "1.2" } };

                // Data in
                Console.WriteLine("Sending records...");
                stream.AppendData(new DataTable(recordsToStream));
                Console.WriteLine("Data successfully streamed to Grok!");

                // Wait a few seconds...
                Console.WriteLine("Waiting for Grok to process...");
                model.WaitForStabilization(true);

                // Predictions out
                DataTable output = model.RetrieveData();

                // Write results out to a CSV
                DirectoryInfo outputDirectory = new DirectoryInfo("output");

                // Create that directory if it doesn't exist
                if (!outputDirectory.Exists)
                    outputDirectory.Create();

                String outputFile = "output/ModelOutput.csv";
                Console.WriteLine("Saving results to " + outputFile);

                output.WriteCSV(new FileInfo(outputFile));

                Console.WriteLine("You can now inspect your results. \n \n");

                Console.WriteLine("Well done! That's it for now!");

            }
            catch (Exception ex)
            {
                // Display encountered errors on console
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
        }
    }
}
