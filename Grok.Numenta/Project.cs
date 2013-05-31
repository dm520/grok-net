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
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>    
    /// The Project class represents the Project object in the API, a logical 
    /// means of grouping Models and Streams within a User account.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Project
    {
        #region Members and Accessors
        #region Member Variables
        private IAPIClient _Client;
	    private string _ID;
	    private string _URL;
	    private string _Name;
	    private string _StreamsUrl;
	    private string _ModelsUrl;
	    private DateTime _CreatedAt;
	    private DateTime _LastUpdated;	
        #endregion Member Variables   
  
        #region Accessors
        [JsonIgnore]
        public IAPIClient ProjectAPIClient 
        {
            get { return _Client; }
            set { _Client = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url
        {
            get { return _URL; }
            set { _URL = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name 
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string streamsUrl
        {
            get { return _StreamsUrl; }
            set { _StreamsUrl = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string modelsUrl
        {
            get { return _ModelsUrl; }
            set { _ModelsUrl = value; }
        }
        
        [JsonIgnore]
        public DateTime createdAt 
        { 
            get { return _CreatedAt; }
        }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt
        {
            set { _CreatedAt = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime lastUpdated 
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }
        #endregion Accessors
        #endregion Members and Accessors

        #region Project Stream Methods
        /// <summary>        
        /// Calls out to the API to create a new Stream in the Project from a given Stream object
	    /// </summary>
	    /// <param name="NewStream"></param>
	    /// <returns></returns>
	    public Stream CreateStream(Stream NewStream)
        {
		    return ProjectAPIClient.CreateStream(this.streamsUrl, NewStream);
	    }
	
	    /// <summary>        
        /// Calls out to the API to retrieve a list of Streams in the Project
	    /// </summary>
	    /// <returns></returns>
	    public List<Stream> RetrieveStreams()
        {
		    return ProjectAPIClient.RetrieveStreams(this.streamsUrl);
	    }
        #endregion Project Stream Methods

        #region Project Model Methods
        /// <summary>        
        /// Calls out to the API to create a new Model in the Project from a given Model object
	    /// </summary>
	    /// <param name="model"></param>
	    /// <returns></returns>
	    public Model CreateModel(Model NewModel)
        {
		    return ProjectAPIClient.CreateModel(this.modelsUrl, NewModel);
	    }

        /// <summary>        
        /// Calls out to the API to retrieve a list of Models in the Project
	    /// </summary>
	    /// <returns></returns>
	    public List<Model> RetrieveModels()
        {
		    return ProjectAPIClient.RetrieveModels(this.modelsUrl);
	    }
        #endregion Project Model Methods

        #region Project Methods
        /// <summary>        
        /// Create a Project locally from a JSON representation (e.g.: as retrieved from the API)
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="JSONObject"></param>
        /// <returns></returns>
        public static Project CreateProject(IAPIClient Client, JObject JSONObject)
        {
            try
            {
                //assume a single project is being passed in
                Project NewProject = JsonConvert.DeserializeObject<Project>(JSONObject["project"].ToString());
                NewProject.ProjectAPIClient = Client;
                return NewProject;
            }
            catch (Exception ex)
            {
                throw new APIException("Could not deserialize the JSON", ex);
            }
        }

        /// <summary>        
        /// Serialize the project object, wrapping it in a "project" group
        /// </summary>
        /// <returns></returns>
        public JObject ToJSON()
        {
            try
            {
                JObject JSONObject = new JObject();
                JSONObject.Add("project", JToken.FromObject(this));
                return JSONObject;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to serialize the JSON", ex);
            }
        }
        #endregion Project Methods
    }
}
