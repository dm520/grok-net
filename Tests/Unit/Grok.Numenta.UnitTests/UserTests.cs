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
    public class UserTests
    {
        string TestJSON = @"{
              ""user"": 
                {
                  ""projectsUrl"": ""https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/projects"", 
                  ""apiKey"": ""API_KEY"", 
                  ""streamsUrl"": ""https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/streams"", 
                  ""firstName"": ""Api"", 
                  ""url"": ""https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980"", 
                  ""lastName"": ""DocUser"", 
                  ""id"": ""0a0acdd4-d293-11e1-bb05-123138107980"", 
                  ""modelsUrl"": ""https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/models"", 
                  ""usageUrl"": ""https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/usage"", 
                  ""tier"": 2, 
                  ""email"": ""apidocs@numenta.com""
                }
              }";

        [Test]
        public void TestCreateUser()
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

            User UserUnderTest = User.CreateUser(MockClient, TestObject);

            Assert.AreEqual(UserUnderTest.id, "0a0acdd4-d293-11e1-bb05-123138107980");
            Assert.AreEqual(UserUnderTest.firstName, "Api");
            Assert.AreEqual(UserUnderTest.lastName, "DocUser");
            Assert.AreEqual(UserUnderTest.email, "apidocs@numenta.com");
            Assert.AreEqual(UserUnderTest.apiKey, "API_KEY");
            Assert.AreEqual(UserUnderTest.modelsUrl, "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/models");
            Assert.AreEqual(UserUnderTest.streamsUrl, "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/streams");
            Assert.AreEqual(UserUnderTest.url, "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980");
            Assert.AreEqual(UserUnderTest.projectsUrl, "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/projects");
            Assert.AreEqual(UserUnderTest.usageUrl, "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/usage");
            Assert.AreEqual(UserUnderTest.UserAPIClient, MockClient);
        }

        [Test]
        public void TestUserToJSON()
        {
            User UserUnderTest = new User();

            UserUnderTest.id = "0a0acdd4-d293-11e1-bb05-123138107980";
            UserUnderTest.firstName = "Api";
            UserUnderTest.lastName = "DocUser";
            UserUnderTest.modelsUrl = "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/models";
            UserUnderTest.streamsUrl = "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/streams";
            UserUnderTest.url = "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980";
            UserUnderTest.projectsUrl = "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/projects";
            UserUnderTest.usageUrl = "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/usage";
            UserUnderTest.apiKey = "API_KEY";
            UserUnderTest.email = "apidocs@numenta.com";

            JObject JSON = UserUnderTest.ToJSON();

            Assert.AreEqual((string)JSON["user"]["id"], "0a0acdd4-d293-11e1-bb05-123138107980");
            Assert.AreEqual((string)JSON["user"]["firstName"], "Api");
            Assert.AreEqual((string)JSON["user"]["lastName"], "DocUser");
            Assert.AreEqual((string)JSON["user"]["email"], "apidocs@numenta.com");
            Assert.AreEqual((string)JSON["user"]["apiKey"], "API_KEY");
            Assert.AreEqual((string)JSON["user"]["modelsUrl"], "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/models");
            Assert.AreEqual((string)JSON["user"]["streamsUrl"], "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/streams");
            Assert.AreEqual((string)JSON["user"]["url"], "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980");
            Assert.AreEqual((string)JSON["user"]["projectsUrl"], "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/projects");
            Assert.AreEqual((string)JSON["user"]["usageUrl"], "https://api.numenta.com/v2/users/0a0acdd4-d293-11e1-bb05-123138107980/usage");
        }
    }
}
