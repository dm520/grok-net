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
    public class DataSourceTests
    {
        [Test]
        public void TestConstructor_SingleStringInput()
        {
            DataSource DSUnderTest = new DataSource("Test Name");

            Assert.AreEqual(DSUnderTest.name, "Test Name");
            Assert.AreEqual(DSUnderTest.dataSourceType, DataSource.TYPE_LOCAL);
        }

        [Test]
        public void TestConstructor_StringStringInput()
        {
            DataSource DSUnderTest = new DataSource("Test Name", DataSource.TYPE_LOCAL);

            Assert.AreEqual(DSUnderTest.name, "Test Name");
            Assert.AreEqual(DSUnderTest.dataSourceType, DataSource.TYPE_LOCAL);
        }

        [Test]
        public void TestConstructor_JSONArrayInput()
        {
            string JSON = @"{ ""dataSources"": [
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

            DataSource DSUnderTest = new DataSource(MockClient, JSONObject);

            Assert.AreEqual(DSUnderTest.name, "My Data Source");
            Assert.AreEqual(DSUnderTest.dataSourceType, DataSource.TYPE_LOCAL);
            Assert.AreEqual(DSUnderTest.fields[0].flag, DataSourceField.FLAG_TIMESTAMP);
            Assert.AreEqual(DSUnderTest.fields[0].dataType, DataSourceField.TYPE_DATETIME);
            Assert.AreEqual(DSUnderTest.fields[0].flag, DataSourceField.FLAG_TIMESTAMP);
        }

        [Test]
        public void TestConstructor_SingleJSONInput()
        {
            string JSON = @"{ ""dataSourceType"": ""local"", 
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

            DataSource DSUnderTest = new DataSource(MockClient, JSONObject);

            Assert.AreEqual(DSUnderTest.name, "My Data Source");
            Assert.AreEqual(DSUnderTest.dataSourceType, DataSource.TYPE_LOCAL);
        }
    }
}
