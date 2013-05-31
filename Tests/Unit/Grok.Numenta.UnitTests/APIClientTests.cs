// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

using Newtonsoft.Json.Linq;

using Grok.Numenta;
using Grok.Numenta.UnitTests;
using System.Diagnostics;

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class APIClientTests
    {
        #region Constructor Test
        [Test]
        public void TestConstructor()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");

            Assert.AreEqual(ClientUnderTest.APIKey, "FAKE_API_KEY");
            Assert.AreEqual(ClientUnderTest.NumentaURI, "https://api.numenta.com/");
        }
        #endregion

        #region Project Tests
        [Test]
        public void TestRetrieveProjects_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(@"{ ""projects"": [
                                                        {
                                                            ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                                                            ""name"": ""Renamed Project"", 
                                                            ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                            ""lastUpdated"": ""2012-08-15T22:13:08Z"", 
                                                            ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                                                            ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                            ""createdAt"": ""2012-08-15T22:12:44Z""
                                                        }
                                                        ]
                                                    }");

            List<Project> Projects = ClientUnderTest.RetrieveProjects("https://api.numenta.com");

            Assert.AreEqual(Projects.Count, 1);
            Assert.AreEqual(Projects[0].name, "Renamed Project");
        }
        

        [Test]
        public void TestRetrieveProjects_BadResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            //pass in an empty string to trigger a HTTP 400 response
            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveProjects(string.Empty); });
        }

        [Test]
        public void TestCreateProject_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(@"{ ""project"": {
                                                                        ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                                                                        ""name"": ""API Doc project for retrieval"", 
                                                                        ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""lastUpdated"": ""2012-08-15T22:12:44Z"", 
                                                                        ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                                                                        ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""createdAt"": ""2012-08-15T22:12:44Z""
                                                                      }
                                                                    }");

            Project EmptyProject = new Project();

            Project FakeProject = ClientUnderTest.CreateProject("https://api.numenta.com", EmptyProject);

            //We test the JSON handling in the Project Tests, so here we just need to ensure that we would have called out to the 
            //service and created a project
            Assert.AreEqual(FakeProject.name, "API Doc project for retrieval");
        }

        [Test]
        public void TestCreateProject_BadResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Project SampleProject = new Project();
            SampleProject.name = "Sample";

            Assert.Throws<APIException>(delegate { ClientUnderTest.CreateProject(string.Empty, SampleProject); });
        }

        [Test]
        public void TestRetrieveProject_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(@"{ ""project"": {
                                                                        ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                                                                        ""name"": ""API Doc project for retrieval"", 
                                                                        ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""lastUpdated"": ""2012-08-15T22:12:44Z"", 
                                                                        ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                                                                        ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""createdAt"": ""2012-08-15T22:12:44Z""
                                                                      }
                                                                    }");

            Project FakeProject = ClientUnderTest.RetrieveProject(string.Empty);

            //We test the JSON handling in the Project Tests, so here we just need to ensure that we would have called out to the 
            //service and retrieved a project
            Assert.AreEqual(FakeProject.name, "API Doc project for retrieval");
        }

        [Test]
        public void TestRetrieveProject_BadResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveProject(string.Empty); });
        }
        #endregion

        #region Stream Tests
        string _SingleStream = @"{ ""stream"": { ""dataUrl"": ""https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1/data"", 
                                                ""name"": ""API Doc stream for retrieval"", 
                                                ""url"": ""https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1"", 
                                                ""projectId"": null, 
                                                ""lastUpdated"": ""2012-08-15T22:12:45Z"", 
                                                ""createdAt"": ""2012-08-15T22:12:45Z"", 
                                                ""id"": ""44cc1b38-317c-4b2f-9f73-7f440351ffa1"", 
                                                ""commandsUrl"": ""https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1/commands"", 
                                                ""dataSources"": [
                                                    {
                                                    ""dataSourceType"": ""local"", 
                                                    ""name"": ""My Data Source"", 
                                                    ""fields"": [
                                                        {
                                                        ""flag"": ""TIMESTAMP"", 
                                                        ""dataFormat"": {
                                                            ""dataType"": ""DATETIME"", 
                                                            ""formatString"": ""sdf/yyyy-MM-dd H:m:s.S""
                                                        }, 
                                                        ""name"": ""timestamp""
                                                        }, 
                                                        {
                                                        ""dataFormat"": {
                                                            ""dataType"": ""SCALAR""
                                                        }, 
                                                        ""name"": ""consumption""
                                                        }
                                                    ]
                                                    }
                                                ]
                                                }
                                            }";

        [Test]
        public void TestCreateStream_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(_SingleStream);

            Stream SampleStream = new Stream();

            Stream FakeStream = ClientUnderTest.CreateStream("https://api.numenta.com", SampleStream);

            //We test the JSON handling in the Stream Tests, so here we just need to ensure that we would have called out to the 
            //service and created a project
            Assert.AreEqual(FakeStream.name, "API Doc stream for retrieval");
        }

        [Test]
        public void TestCreateStream_BadResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Stream SampleStream = new Stream();
            Assert.Throws<APIException>(delegate { ClientUnderTest.CreateStream(string.Empty, SampleStream); });
        }

        [Test]
        public void TestRetrieveStream_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(_SingleStream);

            Stream FakeStream = ClientUnderTest.RetrieveStream(string.Empty);

            //We test the JSON handling in the Stream Tests, so here we just need to ensure that we would have called out to the 
            //service and created a project
            Assert.AreEqual(FakeStream.name, "API Doc stream for retrieval");
        }

        [Test]
        public void TestRetrieveStream_BadResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveStream(string.Empty); });
        }

        [Test]
        public void TestAppendData_GoodResponse()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY", "https://api.numenta.com");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(_SingleStream);

            List<string[]> SampleData = new List<string[]>();
            SampleData.Add(new string[] { "2011-01-01 00:00:00.0", "5.3" });

            SampleUploadCallback Callback = new SampleUploadCallback();
            ClientUnderTest.AppendData("https://api.numenta.com", SampleData, Callback);

            Assert.AreEqual(Callback.PercentComplete, "100%");
        }
        #endregion

        #region Swarm Tests
        string _SwarmJSON = @"{ ""swarm"": {
                                ""status"": ""completed"", 
                                ""url"": ""https://api.numenta.com/v2/swarms/05a6b1da-1040-41d1-9d25-8a1906b97756"", 
                                ""results"": {
                                  ""grokScore"": 5.0, 
                                  ""averageError"": 6.4000000000000004
                                }, 
                                ""lastUpdated"": ""2012-08-15T22:12:29Z"", 
                                ""jobId"": 3052, 
                                ""params"": {
                                  ""size"": ""small""
                                }, 
                                ""details"": {
                                  ""fieldContributions"": [], 
                                  ""numRecords"": 3, 
                                  ""cpuTime"": 1.78, 
                                  ""bestModel"": 35142, 
                                  ""fieldsUsed"": [
                                    ""consumption""
                                  ], 
                                  ""startTime"": ""2012-08-15T22:12:06Z"", 
                                  ""optimizedMetric"": 66.666666666666671, 
                                  ""endTime"": ""2012-08-15T22:12:25Z"", 
                                  ""bestValue"": 66.666666666666671
                                }, 
                                ""id"": ""05a6b1da-1040-41d1-9d25-8a1906b97756"", 
                                ""createdAt"": ""2012-08-15T22:12:06Z"", 
                                ""modelId"": ""f87a8444-8459-41bb-8463-81b83747226e""
                              }
                            }";

        [Test]
        public void TestRetrieveSwarm_GoodRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_CLIENT");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(_SwarmJSON);
            Swarm TestSwarm = ClientUnderTest.RetrieveSwarm(string.Empty);

            Assert.AreEqual(TestSwarm.id, "05a6b1da-1040-41d1-9d25-8a1906b97756");
        }

        [Test]
        public void TestRetrieveSwarm_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_CLIENT");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveSwarm(string.Empty); });
        }

        [Test]
        public void TestRetrieveSwarm_Expires()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_CLIENT");
            ClientUnderTest.HTTPClient = GetHttpClientWith200ResponseAndExpires(_SwarmJSON, DateTimeOffset.UtcNow.AddSeconds(30));

            Swarm TestSwarm = ClientUnderTest.RetrieveSwarm(string.Empty);

            Assert.AreEqual(30, TestSwarm.Expires);
        }

        [Test]
        public void TestCreateSwarm_GoodRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_CLIENT");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(_SwarmJSON);
            Swarm TestSwarm = ClientUnderTest.CreateSwarm("https://api.numenta.com");

            Assert.AreEqual(TestSwarm.id, "05a6b1da-1040-41d1-9d25-8a1906b97756");
        }

        [Test]
        public void TestCreateSwarm_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_CLIENT");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.CreateSwarm(string.Empty); });
        }
        #endregion

        #region Model Tests
        [Test]
        public void TestSendModelCommand_GoodRequest()
        {
            string Response = "{ \"ok\": true }";
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(Response);
            Dictionary<string, string> Parameters = new Dictionary<string, string>();
            Parameters["fake_param"] = "fake_value";

            JObject Value = ClientUnderTest.SendModelCommand("https://api.numenta.com", "promote", Parameters);

            Assert.AreEqual((bool)Value["ok"], true);
        }

        [Test]
        public void TestSendModelCommand_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();
            
            Assert.Throws<APIException>(delegate { ClientUnderTest.SendModelCommand(string.Empty, "promote"); });
        }

        [Test]
        public void TestRetrieveOutputData_GoodRequest()
        {
            string Response = @"{ ""output"": {
                                    ""meta"": {
                                      ""engineModelId"": 35149, 
                                      ""timestampIndex"": 1, 
                                      ""engineJobId"": 3054, 
                                      ""predictedFieldPredictionIndex"": 7, 
                                      ""modelStatus"": ""running"", 
                                      ""predictedFieldIndex"": 2
                                    }, 
                                    ""data"": [
                                      [
                                        0, 
                                        ""2010-10-01 00:00:00"", 
                                        ""20.5"", 
                                        ""0"", 
                                        ""0"", 
                                        20.5, 
                                        ""{1: {20.5: 1.0}}"", 
                                        ""20.5"", 
                                        20.5, 
                                        20.5, 
                                        20.5
                                      ], 
                                      [
                                        1, 
                                        ""2010-10-01 01:00:00"", 
                                        ""11.5"", 
                                        ""0.0"", 
                                        ""0.0"", 
                                        11.5, 
                                        ""{1: {11.5: 0.99607843137254592, 20.5: 0.0039215686274509803}}"", 
                                        ""11.5"", 
                                        11.5, 
                                        20.5, 
                                        20.5
                                      ]
                                    ], 
                                    ""names"": [
                                      ""ROWID"", 
                                      ""timestamp"", 
                                      ""consumption"", 
                                      ""Grok Score"", 
                                      ""Average Grok Score"", 
                                      ""Best Prediction"", 
                                      ""Prediction Details"", 
                                      ""Predicted consumption"", 
                                      ""Step 1 Rank 1"", 
                                      ""Step 1 Rank 2"", 
                                      ""Step 1 Rank 3""
                                    ]
                                  }, 
                                  ""ok"": true
                                }";
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(Response);

            DataTable Data = ClientUnderTest.RetrieveOutputData(string.Empty);

            Assert.AreEqual(Data.Names[0], "ROWID");
        }

        [Test]
        public void TestRetrieveOutputData_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveOutputData(string.Empty); });
        }

        [Test]
        public void TestCreateModel_GoodRequest()
        {
            string Response = @"{ ""model"": {
                                    ""status"": """", 
                                    ""dataUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/data"", 
                                    ""name"": ""API Doc model for retrieval"", 
                                    ""predictedField"": ""consumption"", 
                                    ""url"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002"", 
                                    ""projectId"": null, 
                                    ""swarmsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/swarms"", 
                                    ""lastUpdated"": ""2012-08-15T22:12:47Z"", 
                                    ""createdAt"": ""2012-08-15T22:12:47Z"", 
                                    ""commandsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/commands"", 
                                    ""streamId"": ""44cc1b38-317c-4b2f-9f73-7f440351ffa1"", 
                                    ""id"": ""1fcaea6a-a1ac-4320-8b3f-d20511e6b002"", 
                                    ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints""
                                  }
                                }";
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(Response);

            Model TestModel = new Model();
            Model ReturnModel = ClientUnderTest.CreateModel("https://api.numenta.com", TestModel);

            Assert.AreEqual(ReturnModel.name, "API Doc model for retrieval");
        }

        [Test]
        public void TestCreateModel_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();
            Model TestModel = new Model();

            Assert.Throws<APIException>(delegate { ClientUnderTest.CreateModel(string.Empty, TestModel); });
        }

        [Test]
        public void TestRetrieveModel_GoodRequest()
        {
            string Response = @"{ ""model"": {
                                    ""status"": """", 
                                    ""dataUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/data"", 
                                    ""name"": ""API Doc model for retrieval"", 
                                    ""predictedField"": ""consumption"", 
                                    ""url"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002"", 
                                    ""projectId"": null, 
                                    ""swarmsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/swarms"", 
                                    ""lastUpdated"": ""2012-08-15T22:12:47Z"", 
                                    ""createdAt"": ""2012-08-15T22:12:47Z"", 
                                    ""commandsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/commands"", 
                                    ""streamId"": ""44cc1b38-317c-4b2f-9f73-7f440351ffa1"", 
                                    ""id"": ""1fcaea6a-a1ac-4320-8b3f-d20511e6b002"", 
                                    ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints""
                                  }
                                }";
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith200Response(Response);

            Model ReturnModel = ClientUnderTest.RetrieveModel(string.Empty);

            Assert.AreEqual(ReturnModel.name, "API Doc model for retrieval");
        }

        [Test]
        public void TestRetrieveModel_BadRequest()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith400Response();

            Assert.Throws<APIException>(delegate { ClientUnderTest.RetrieveModel(string.Empty); });
        }


        [Test]
        public void TestDeleteModel()
        {
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY");
            ClientUnderTest.HTTPClient = GetHttpClientWith204Response();
            ClientUnderTest.DeleteModel("http://localhost:8888");

        }
        #endregion

        #region Connection Tests
        [Test]
        public void TestRetryAfterNoHeader()
        {
            HttpResponseMessage goodResponse = new HttpResponseMessage(HttpStatusCode.OK);
            goodResponse.Content = new StringContent(@"{ ""project"": {
                                                                        ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                                                                        ""name"": ""API Doc project for retrieval"", 
                                                                        ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""lastUpdated"": ""2012-08-15T22:12:44Z"", 
                                                                        ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                                                                        ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""createdAt"": ""2012-08-15T22:12:44Z""
                                                                      }
                                                                    }");

            // Simulate Service Unavailable error (503) recovering on the 3rd retry
            HttpClient HTTPClient = new HttpClient(new FakeRetryHandler()
            {
                Retries = 3,
                Response = goodResponse,
                BadResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
                InnerHandler = new HttpClientHandler()
            });
            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY") { Retry = 3, Timeout = 30 };
            ClientUnderTest.HTTPClient = HTTPClient;

            // Try to connect and get a fake project
            Project project = ClientUnderTest.RetrieveProject("https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55");
            Assert.AreEqual("dbbfc567-c409-4e7a-b06f-941f752e2f55", project.id);
        }
        [Test]
        public void TestRetryAfterWithHeader()
        {
            HttpResponseMessage goodResponse = new HttpResponseMessage(HttpStatusCode.OK);
            goodResponse.Content = new StringContent(@"{ ""project"": {
                                                                        ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                                                                        ""name"": ""API Doc project for retrieval"", 
                                                                        ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""lastUpdated"": ""2012-08-15T22:12:44Z"", 
                                                                        ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                                                                        ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                                                                        ""createdAt"": ""2012-08-15T22:12:44Z""
                                                                      }
                                                                    }");

            HttpResponseMessage badResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            badResponse.Headers.Add("Retry-After", "30"); // Tell client to retry after 30 seconds

            // Simulate Bad Gateway and recovery on the second time
            HttpClient HTTPClient = new HttpClient(new FakeRetryHandler()
            {
                Retries = 2, 
                Response = goodResponse,
                BadResponse = badResponse,
                InnerHandler = new HttpClientHandler()
            });

            APIClient ClientUnderTest = new APIClient("FAKE_API_KEY") { Retry = 0, Timeout = 30 };
            ClientUnderTest.HTTPClient = HTTPClient;

            // Try to connect and get a fake project
            Stopwatch timer = Stopwatch.StartNew();
            Project project = ClientUnderTest.RetrieveProject("https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55");
            timer.Stop();
            Assert.AreEqual("dbbfc567-c409-4e7a-b06f-941f752e2f55", project.id);

            // The first request will return Service Unavailable error (503) with "Retry-After" 30 seconds.
            // The client should wait at least 30 seconds before retrying.
            Assert.GreaterOrEqual((timer.ElapsedMilliseconds + 500)/1000, 30);
        }
        #endregion

        #region Helper Methods
        private HttpClient GetHttpClientWith400Response()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
                                                        {
                                                            Response = response,
                                                            InnerHandler = new HttpClientHandler()
                                                        });
            return HTTPClient;
        }

        private HttpClient GetHttpClientWith200Response(string RequestContent)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(RequestContent);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });
            return HTTPClient;
        }

        private HttpClient GetHttpClientWith204Response()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });
            return HTTPClient;
        }
        private HttpClient GetHttpClientWith200ResponseAndExpires(string RequestContent, DateTimeOffset expires)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(RequestContent);
            response.Content.Headers.Expires = expires;
            HttpClient HTTPClient = new HttpClient(new FakeHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });
            return HTTPClient;
        }
        #endregion
    }
}
