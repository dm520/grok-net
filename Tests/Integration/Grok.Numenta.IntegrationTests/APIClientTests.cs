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

namespace Grok.Numenta.IntegrationTests
{
    [TestFixture]
    public class APIClientTests
    {
        #region Constructor Test
        private static string API_KEY = System.Configuration.ConfigurationManager.AppSettings["APIKey"];
        private static string BaseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"];
        private static string Version = "v2/";
        
        [Test]
        public void TestConstructor()
        {
            APIClient ClientUnderTest = new APIClient(API_KEY, BaseURL);

            Assert.AreEqual(ClientUnderTest.APIKey, API_KEY);
            Assert.AreEqual(ClientUnderTest.NumentaURI, BaseURL);
            Assert.AreEqual(ClientUnderTest.DefaultUser.UserAPIClient, ClientUnderTest);
            Assert.AreEqual(ClientUnderTest.DefaultUser.apiKey, API_KEY);
        }
        #endregion

        #region Setup and Teardown
        [SetUp]
        public void SetUpTest()
        {
            if (!BaseURL.EndsWith("/"))
                BaseURL += "/";
        }
        #endregion

        #region Integration Tests
        [Test]
        public void ProjectIntegrationTests()
        {
            APIClient ClientUnderTest = new APIClient(API_KEY, BaseURL);

            //Create a Test Project
            Project ProjectUnderTest = ClientUnderTest.CreateProject("Project for Integration Testing");
            Assert.AreEqual(ClientUnderTest.DefaultUser.apiKey, API_KEY);
            string ProjectID = ProjectUnderTest.id;
            Assert.AreEqual(ProjectUnderTest.name, "Project for Integration Testing");
            Assert.AreEqual(ProjectUnderTest.url, BaseURL + Version + "projects/" + ProjectID);

            //Test Retrieve Projects
            List<Project> AllUserProjects = ClientUnderTest.RetrieveProjects();
            bool FoundProject = false;
            foreach (Project p in AllUserProjects)
            {
                //there might be other projects, so we'll just ignore those
                if (p.id == ProjectUnderTest.id)
                {
                    Assert.AreEqual(p.modelsUrl, ProjectUnderTest.modelsUrl);
                    Assert.AreEqual(p.streamsUrl, ProjectUnderTest.streamsUrl);
                    Assert.AreEqual(p.url, ProjectUnderTest.url);
                    Assert.AreEqual(p.name, ProjectUnderTest.name);
                    FoundProject = true;
                    break;
                }
            }
            Assert.IsTrue(FoundProject);

            Assert.IsTrue(ClientUnderTest.DeleteProject(ProjectUnderTest));
        }
        #endregion Integration Tests
    }
}
