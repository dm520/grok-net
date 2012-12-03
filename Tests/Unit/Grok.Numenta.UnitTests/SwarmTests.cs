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
using NUnit.Framework;

using Newtonsoft.Json.Linq;

using Grok.Numenta;

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class SwarmTests
    {
        [Test]
        public void TestCreateSwarm()
        {
            string JSON = @"{
                              ""swarm"": {
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
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(JSON);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
                                                        {
                                                            Response = response,
                                                            InnerHandler = new HttpClientHandler()
                                                        });

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            MockClient.HTTPClient = HTTPClient;

            JObject JSONObject = JObject.Parse(JSON);

            Swarm SwarmUnderTest = Swarm.CreateSwarm(MockClient, JSONObject);

            Assert.AreEqual(SwarmUnderTest.id, "05a6b1da-1040-41d1-9d25-8a1906b97756");
            Assert.AreEqual(SwarmUnderTest.status, "completed");
            Assert.AreEqual(SwarmUnderTest.url, "https://api.numenta.com/v2/swarms/05a6b1da-1040-41d1-9d25-8a1906b97756");
            Assert.AreEqual((JArray)SwarmUnderTest.details["fieldContributions"], new JArray());
            Assert.AreEqual((int)SwarmUnderTest.details["numRecords"], 3);
            Assert.AreEqual((double)SwarmUnderTest.details["cpuTime"], 1.78);
            Assert.AreEqual((int)SwarmUnderTest.details["bestModel"], 35142);
            Assert.AreEqual((string)SwarmUnderTest.details["fieldsUsed"][0], "consumption");
            Assert.AreEqual(SwarmUnderTest.SwarmAPIClient, MockClient);
        }
    }
}
