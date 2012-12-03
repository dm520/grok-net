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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 19 November 2012
    /// Class: User
    /// Description: The <code>User</code> class represents a user account.
    /// It can be used to perform basic retrieval and update operations.
    /// Every valid API key is associated with at least one user account.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class User
    {

        #region Members and Accessors
        #region Member Data
        private IAPIClient _Client;
        private string _ID;
        private string _FirstName;
        private string _LastName;
        private string _Email;
        private string _APIKey;
        private string _URL;
        private string _ProjectsURL;
        private string _StreamsURL;
        private string _ModelsURL;
        private string _UsageURL;
        private string _Tier;
        #endregion Member Data

        #region Accessors
        [JsonIgnore]
        public IAPIClient UserAPIClient
        {
            get { return _Client; }
            set { _Client = value; }
        }

        [JsonProperty]
        public string apiKey
        {
            get { return _APIKey; }
            set { _APIKey = value; }
        }

        [JsonProperty]
        public string id
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [JsonProperty]
        public string firstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }

        [JsonProperty]
        public string lastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        [JsonProperty]
        public string email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        [JsonProperty]
        public string url
        {
            get { return _URL; }
            set { _URL = value; }
        }

        [JsonProperty]
        public string projectsUrl
        {
            get { return _ProjectsURL; }
            set { _ProjectsURL = value; }
        }

        [JsonProperty]
        public string streamsUrl
        {
            get { return _StreamsURL; }
            set { _StreamsURL = value; }
        }

        [JsonProperty]
        public string modelsUrl
        {
            get { return _ModelsURL; }
            set { _ModelsURL = value; }
        }

        [JsonProperty]
        public string usageUrl
        {
            get { return _UsageURL; }
            set { _UsageURL = value; }
        }

        [JsonIgnore]
        public string tier
        {
            get { return _Tier; }
            set { _Tier = value; }
        }
        #endregion Accessors
        #endregion Members and Accessors
        
        #region User Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateUser
        /// Description: Creates a new user based on API client and JSON representation.
        /// This constructor is intended to be used only be the API client directly.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="JSONObject"></param>
        /// <returns></returns>
        public static User CreateUser(IAPIClient Client, JObject JSONObject)
        {
            try
            {
                //assume a single user is being passed in
                User CurrentUser = JsonConvert.DeserializeObject<User>(JSONObject["user"].ToString());
                CurrentUser.UserAPIClient = Client;
                return CurrentUser;
            }
            catch (Exception ex)
            {
                throw new APIException("Could not deserialize the JSON", ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: Update
        /// Description: Calls into the API to update the User record
        /// </summary>
        public void Update()
        {
            UserAPIClient.UpdateUser(this);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: ToJSON
        /// Description: Returns a JSON representation of the User
        /// </summary>
        /// <returns>A JSON Representation of the User that can be fed into the Grok REST API</returns>
        public JObject ToJSON()
        {
            try
            {
                JObject JSONObject = new JObject();
                JSONObject.Add("user", JToken.FromObject(this));
                return JSONObject;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to serialize the JSON", ex);
            }
        }
        #endregion User Methods
        
        #region User Projects
        /// <summary>
        ///Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateProject
        /// Description:  Creates a new project from a Project object
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Project CreateProject(Project NewProject)
        {
            return UserAPIClient.CreateProject(this.projectsUrl, NewProject);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateProject
        /// Description: Create an empty Project with the given name
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public Project CreateProject(string ProjectName)
        {
            Project NewProject = new Project();
            NewProject.name = ProjectName;
            return UserAPIClient.CreateProject(this.projectsUrl, NewProject);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveProjects
        /// Description: Calls into the API to return the list of Projects for a User account
        /// </summary>
        /// <returns></returns>
        public List<Project> RetrieveProjects()
        {
            return UserAPIClient.RetrieveProjects(this.projectsUrl);
        }
        #endregion User Projects

        #region User Streams
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveStreams
        /// Description: Calls into the API to retrieve a list of Streams for the User account that are not associated to Projects
        /// </summary>
        /// <returns></returns>
        public List<Stream> RetrieveStreams()
        {
            return UserAPIClient.RetrieveStreams(this.streamsUrl);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveAllStreams
        /// Description: Calls into the API to retrieve a list of Streams for the User account, irrespective of association to Projects
        /// </summary>
        /// <returns></returns>
        public List<Stream> RetrieveAllStreams()
        {
            return UserAPIClient.RetrieveStreams(this.streamsUrl + "?all=true");
        }
        #endregion User Streams
    }
}