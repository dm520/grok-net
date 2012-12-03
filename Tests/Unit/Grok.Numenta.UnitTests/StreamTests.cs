// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using NUnit.Framework;

using Newtonsoft.Json.Linq;

using Grok.Numenta;

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class StreamTests
    {
        [Test]
        public void TestCreateStream()
        {
            string JSON = @"{
                              ""stream"": {
                                ""dataUrl"": ""https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1/data"", 
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
            JObject JSONObject = JObject.Parse(JSON);

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JSON);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
                                                        {
                                                            Response = response,
                                                            InnerHandler = new HttpClientHandler()
                                                        });

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            MockClient.HTTPClient = HTTPClient;

            Stream StreamUnderTest = Stream.CreateStream(MockClient, JSONObject);

            Assert.AreEqual(StreamUnderTest.id, "44cc1b38-317c-4b2f-9f73-7f440351ffa1");
            Assert.AreEqual(StreamUnderTest.name, "API Doc stream for retrieval");
            Assert.AreEqual(StreamUnderTest.url, "https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1");
            Assert.AreEqual(StreamUnderTest.dataUrl, "https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1/data");
            Assert.AreEqual(StreamUnderTest.commandsUrl, "https://api.numenta.com/v2/streams/44cc1b38-317c-4b2f-9f73-7f440351ffa1/commands");
            Assert.AreEqual((DateTime)StreamUnderTest.lastUpdated, DateTime.Parse("2012-08-15T22:12:45"));
            Assert.AreEqual((string)StreamUnderTest.dataSources[0].dataSourceType, DataSource.TYPE_LOCAL);
            Assert.AreEqual((string)StreamUnderTest.dataSources[0].name, "My Data Source");
            Assert.AreEqual(((Dictionary<string, string>)StreamUnderTest.dataSources[0].fields[0].dataFormat)["dataType"], DataSourceField.TYPE_DATETIME);
            Assert.AreEqual(((Dictionary<string, string>)StreamUnderTest.dataSources[0].fields[0].dataFormat)["formatString"], "sdf/yyyy-MM-dd H:m:s.S");
            Assert.AreEqual((string)StreamUnderTest.dataSources[0].fields[0].flag, DataSourceField.FLAG_TIMESTAMP);
            Assert.AreEqual((string)StreamUnderTest.dataSources[0].fields[0].name, "timestamp");
            Assert.AreEqual(((Dictionary<string, string>)StreamUnderTest.dataSources[0].fields[1].dataFormat)["dataType"], DataSourceField.TYPE_SCALAR);
            Assert.AreEqual((string)StreamUnderTest.dataSources[0].fields[1].name, "consumption");
            Assert.AreEqual(StreamUnderTest.StreamAPIClient, MockClient);
        }

        [Test]
        public void TestToJSON_NewStreamForCreation()
        {
            DataSourceField timestamp = new DataSourceField(
                        "timestamp",
                        DataSourceField.TYPE_DATETIME,
                        DataSourceField.FLAG_TIMESTAMP);

            DataSourceField consumption = new DataSourceField(
                    "consumption",
                    DataSourceField.TYPE_SCALAR);

            DataSource dataSource = new DataSource();
            dataSource.name = "My Data Source";
            dataSource.dataSourceType = "local";
            dataSource.fields.Add(timestamp);
            dataSource.fields.Add(consumption);

            Grok.Numenta.Stream StreamUnderTest = new Grok.Numenta.Stream();
            StreamUnderTest.name = "My Stream";
            StreamUnderTest.dataSources.Add(dataSource);

            JObject JSON = StreamUnderTest.ToJSON();

            Assert.AreEqual((string)JSON["stream"]["name"], "My Stream");
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["dataSourceType"], DataSource.TYPE_LOCAL);
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["name"], "My Data Source");
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["fields"][0]["flag"], DataSourceField.FLAG_TIMESTAMP);
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["fields"][0]["dataFormat"]["dataType"], DataSourceField.TYPE_DATETIME);
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["fields"][0]["name"], "timestamp");
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["fields"][1]["dataFormat"]["dataType"], DataSourceField.TYPE_SCALAR);
            Assert.AreEqual((string)JSON["stream"]["dataSources"][0]["fields"][1]["name"], "consumption");
        }
    }
}
