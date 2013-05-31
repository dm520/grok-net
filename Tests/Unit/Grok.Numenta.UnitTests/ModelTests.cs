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
                                ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints"",
                                ""customErrorMetric"": {
                                    ""errorWindow"": ""1000"",
                                    ""customExpr"": ""100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0""}
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
            Assert.IsNotNull(ModelUnderTest.customErrorMetric);
            Assert.AreEqual(ModelUnderTest.customErrorMetric["errorWindow"], "1000");
            Assert.AreEqual(ModelUnderTest.customErrorMetric["customExpr"], "100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0");
        }

        [Test]
        public void TestGetLabel()
        {
            string modelJSONStr =
                           @"{
                              ""model"": {
                                ""modelType"":""anomalyDetector"",
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
                                ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints"",
                                ""customErrorMetric"": {
                                    ""errorWindow"": ""1000"",
                                    ""customExpr"": ""100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0""}
                              }
                            }";
            string getLabelResponse =
                        @"{
                            ""isProcessing"": false, 
                            ""recordLabels"": [
                                                {""ROWID"" : 1000,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1001,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1002,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1003,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1004,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1005,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1006,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1007,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1008,""labels"" : [""HelloAnomaly""]},
                                                {""ROWID"" : 1009,""labels"" : [""HelloAnomaly""]}                                
                                                ], 
                         }";
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(getLabelResponse);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            MockClient.HTTPClient = HTTPClient;

            JObject modelJSONObj = JObject.Parse(modelJSONStr);
            Model model = Model.CreateModel(MockClient, modelJSONObj);
            
            var labels = model.GetLabels();
            Assert.IsTrue(labels.ContainsKey(1005));
            Assert.AreEqual(labels[1005][0], "HelloAnomaly");

        }
        [Test]
        public void TestGetAnomalyWaitRescords()
        {
            string modelJSONStr =
                           @"{
                              ""model"": {
                                ""modelType"":""anomalyDetector"",
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
                                ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints"",
                                ""customErrorMetric"": {
                                    ""errorWindow"": ""1000"",
                                    ""customExpr"": ""100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0""}
                              }
                            }";

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("{\"autoDetectWaitRecords\": 1000}");
            HttpClient HTTPClient = new HttpClient(new FakeHandler
            {
                Response = response,
                InnerHandler = new HttpClientHandler()
            });

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            MockClient.HTTPClient = HTTPClient;

            JObject modelJSONObj = JObject.Parse(modelJSONStr);
            Model model = Model.CreateModel(MockClient, modelJSONObj);
            Assert.AreEqual(1000,model.GetAutoDetectWaitRecords());
        }
        [Test]
        public void TestSetComputeInterval()
        {
            string modelJSONStr =
                           @"{
                              ""model"": {
                                ""modelType"":""anomalyDetector"",
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
                                ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints"",
                                ""customErrorMetric"": {
                                    ""errorWindow"": ""1000"",
                                    ""customExpr"": ""100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0""}
                              }
                            }";

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            JObject modelJSONObj = JObject.Parse(modelJSONStr);
            Model model = Model.CreateModel(MockClient, modelJSONObj);
            model.SetComputeInterval(1, Model.ComputeInterval.Hours);
            Assert.AreEqual(model.compute["interval"]["hours"].Value<int>(), 1);

        }
        [Test]
        public void TestIsLearning()
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
                                ""learning"":""enabled"",
                                ""commandsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/commands"", 
                                ""streamId"": ""44cc1b38-317c-4b2f-9f73-7f440351ffa1"", 
                                ""id"": ""1fcaea6a-a1ac-4320-8b3f-d20511e6b002"", 
                                ""checkpointsUrl"": ""https://api.numenta.com/v2/models/1fcaea6a-a1ac-4320-8b3f-d20511e6b002/checkpoints"",
                                ""customErrorMetric"": {
                                    ""errorWindow"": ""1000"",
                                    ""customExpr"": ""100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0""}
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
            Assert.IsTrue(ModelUnderTest.isLearning);
            ModelUnderTest.DisableLearning();
            Assert.IsFalse(ModelUnderTest.isLearning);

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
            HelloGrokModel.customErrorMetric = new Dictionary<string, Object>();
            HelloGrokModel.customErrorMetric.Add("errorWindow", 1000);
            HelloGrokModel.customErrorMetric.Add("customExpr", "100.0 * abs(prediction - groundTruth)/groundTruth \n if groundTruth != 0 else 0");
            HelloGrokModel.SetComputeInterval(1, Model.ComputeInterval.Hours);

            JObject JSONObject = HelloGrokModel.ToJSON();

            Assert.IsNull(JSONObject["model"]["status"]);
            Assert.AreEqual((string)JSONObject["model"]["streamId"], "FAKE_STREAM_ID");
            Assert.AreEqual((string)JSONObject["model"]["name"], "Test Name");
            Assert.AreEqual((string)JSONObject["model"]["predictedField"], "consumption");
            Assert.AreEqual((string)JSONObject["model"]["aggregation"]["fields"][0][0], "consumption");
            Assert.AreEqual((string)JSONObject["model"]["aggregation"]["fields"][0][1], TimeAggregation.FUNCTION_MEAN);
            Assert.AreEqual((int)JSONObject["model"]["customErrorMetric"]["errorWindow"], 1000);
            Assert.AreEqual((int)JSONObject["model"]["compute"]["interval"]["hours"], 1);

        }
    }
}
