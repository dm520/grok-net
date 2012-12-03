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
    public class ModelTests
    {
        [Test]
        public void TestCreateModel()
        {
            string JSON = @"{
                              ""model"": {
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

            Model ModelUnderTest = Model.CreateModel(MockClient, JSONObject);

            Assert.AreEqual(ModelUnderTest.ModelAPIClient, MockClient);
            Assert.AreEqual(ModelUnderTest.status, string.Empty);
            Assert.AreEqual(ModelUnderTest.dataUrl, "https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/data");
            Assert.AreEqual(ModelUnderTest.name, "API Doc model for retrieval");
            Assert.AreEqual(ModelUnderTest.predictedField, "consumption");
            Assert.AreEqual(ModelUnderTest.swarmsUrl, "https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/swarms");
            Assert.AreEqual(ModelUnderTest.url, "https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002");
            Assert.AreEqual(ModelUnderTest.lastUpdated, DateTime.Parse("2012-08-15T22:12:47"));
            Assert.AreEqual(ModelUnderTest.commandsUrl, "https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/commands");
            Assert.AreEqual(ModelUnderTest.streamId, "44cc1b38-317c-4b2f-9f73-7f440351ffa1");
            Assert.AreEqual(ModelUnderTest.id, "1fcaea6a-a1ac-4320-8b3f-d20511e6b002");
            Assert.AreEqual(ModelUnderTest.checkpointsUrl, "https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints");
        }

        [Test]
        public void TestToJSON()
        {
            TimeAggregation aggregation = new TimeAggregation();
            aggregation.SetHours(1);
            aggregation.FieldOverrides.Add(new string[] { "consumption", TimeAggregation.FUNCTION_MEAN });

            Model HelloGrokModel = new Model();
            HelloGrokModel.name = "Test Name";
            HelloGrokModel.streamId = "FAKE_STREAM_ID";
            HelloGrokModel.predictedField = "consumption";
            HelloGrokModel.aggregation = aggregation;

            JObject JSONObject = HelloGrokModel.ToJSON();

            Assert.IsNull(JSONObject["model"]["status"]);
            Assert.AreEqual((string)JSONObject["model"]["streamId"], "FAKE_STREAM_ID");
            Assert.AreEqual((string)JSONObject["model"]["name"], "Test Name");
            Assert.AreEqual((string)JSONObject["model"]["predictedField"], "consumption");
            Assert.AreEqual((string)JSONObject["model"]["aggregation"]["fields"][0][0], "consumption");
            Assert.AreEqual((string)JSONObject["model"]["aggregation"]["fields"][0][1], TimeAggregation.FUNCTION_MEAN);
        }
    }
}
