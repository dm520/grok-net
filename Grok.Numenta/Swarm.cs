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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>    
    /// The Swarm class represents a Swarm.
    /// It can be used to perform basic retrieval and update operations.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Swarm
    {
        #region Private Members and Accessors
        #region Member Variables
        public const string SIZE_SMALL = "small";
	    public const string SIZE_MEDIUM = "medium";
        public const string SIZE_LARGE = "large";
	
	    private IAPIClient _Client;
        private string _ID;
        private string _URL;
        private string _Status;
        private JObject _Details;
        private int _Expires = 0;
        #endregion Member Variables

        #region Accessors
        [JsonIgnore]
        public IAPIClient SwarmAPIClient
        {
            get { return _Client; }
            set { _Client = value; }
        }
        /// <summary>
        /// While the swarn is running, return the number of seconds to wait before
        /// checking if the swarm process is complete. Default value:0
        /// </summary>
        [JsonIgnore]
        public int Expires
        {
            get { return _Expires; }
            set { _Expires = value; }
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
        public string status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject details
        {
            get { return _Details; }
            set { _Details = value; }
        }
        #endregion Accessors
        #endregion Private Members and Accessors

        #region Constructors
        /// <summary>        
        /// Default Constructor
        /// </summary>
        public Swarm() { }

        /// <summary>
        ///  Author: Jared Casner
        
        
        /// Deserialize a JSON Object from the API into a Swarm object
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="JSONObject"></param>
        /// <returns></returns>
	    public static Swarm CreateSwarm(IAPIClient Client, JObject JSONObject) 
        {
            try
            {
                string key = string.Empty;
                Swarm NewSwarm = new Swarm();
                if (((JProperty)JSONObject.First).Name.Equals("swarm"))
                    NewSwarm = JsonConvert.DeserializeObject<Swarm>(JSONObject["swarm"].ToString());
                else 
                    NewSwarm = JsonConvert.DeserializeObject<List<Swarm>>(JSONObject["swarms"].ToString())[0];
                
                NewSwarm.SwarmAPIClient = Client;
                JToken expire = JSONObject.GetValue("Expires");
                if (expire != null)
                {
                    NewSwarm.Expires = expire.Value<int>();
                }
                return NewSwarm;
            }
            catch (Exception ex)
            {
                throw new APIException("Could not deserialize the JSON", ex);
            }
	    }
        #endregion

        #region Helper Methods
        /// <summary>
        ///  Author: Jared Casner
        
        
        /// Returns a Swarm from the API given the URL
        /// </summary>
        /// <returns></returns>
        public Swarm Retrieve() 
        {
		    return SwarmAPIClient.RetrieveSwarm(this.url);
        }
        #endregion
    }
}
