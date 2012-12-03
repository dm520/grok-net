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
    public class ProjectTests
    {
        string TestJSON = @"{
                      ""project"": {
                        ""streamsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams"", 
                        ""name"": ""API Doc project for retrieval"", 
                        ""url"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                        ""lastUpdated"": ""2012-08-15T22:12:44Z"", 
                        ""modelsUrl"": ""https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models"", 
                        ""id"": ""dbbfc567-c409-4e7a-b06f-941f752e2f55"", 
                        ""createdAt"": ""2012-08-15T22:12:44Z""
                      }
                    }";

        [Test]
        public void TestCreateProject()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(TestJSON);
            HttpClient HTTPClient = new HttpClient(new FakeHandler
                                                        {
                                                            Response = response,
                                                            InnerHandler = new HttpClientHandler()
                                                        });

            APIClient MockClient = new APIClient("FAKE_API_KEY");
            MockClient.HTTPClient = HTTPClient;

            JObject TestObject = JObject.Parse(TestJSON);

            Project ProjectUnderTest = Project.CreateProject(MockClient, TestObject);

            Assert.AreEqual(ProjectUnderTest.id, "dbbfc567-c409-4e7a-b06f-941f752e2f55");
            Assert.AreEqual(ProjectUnderTest.name, "API Doc project for retrieval");
            Assert.AreEqual(ProjectUnderTest.lastUpdated, DateTime.Parse("2012-08-15T22:12:44"));
            Assert.AreEqual(ProjectUnderTest.modelsUrl, "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models");
            Assert.AreEqual(ProjectUnderTest.streamsUrl, "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams");
            Assert.AreEqual(ProjectUnderTest.url, "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55");
            Assert.AreEqual(ProjectUnderTest.createdAt, DateTime.Parse("2012-08-15T22:12:44"));
            Assert.AreEqual(ProjectUnderTest.ProjectAPIClient, MockClient);
        }

        [Test]
        public void TestProjectToJSON()
        {
            Project ProjectUnderTest = new Project();

            ProjectUnderTest.id = "dbbfc567-c409-4e7a-b06f-941f752e2f55";
            ProjectUnderTest.name = "API Doc project for retrieval";
            ProjectUnderTest.lastUpdated = DateTime.Parse("2012-08-15T22:12:44");
            ProjectUnderTest.modelsUrl = "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models";
            ProjectUnderTest.streamsUrl = "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams";
            ProjectUnderTest.url = "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55";

            JObject JSON = ProjectUnderTest.ToJSON();

            Assert.AreEqual((string)JSON["project"]["id"], "dbbfc567-c409-4e7a-b06f-941f752e2f55");
            Assert.AreEqual((string)JSON["project"]["name"], "API Doc project for retrieval");
            Assert.AreEqual((string)JSON["project"]["createdAt"], null);
            Assert.AreEqual((DateTime)JSON["project"]["lastUpdated"], DateTime.Parse("2012-08-15T22:12:44"));
            Assert.AreEqual((string)JSON["project"]["modelsUrl"], "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/models");
            Assert.AreEqual((string)JSON["project"]["streamsUrl"], "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55/streams");
            Assert.AreEqual((string)JSON["project"]["url"], "https://api.numenta.com/v2/projects/dbbfc567-c409-4e7a-b06f-941f752e2f55");
        }
    }
}
