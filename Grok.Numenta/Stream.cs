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
    /// The Stream class represents a Stream.
    /// It can be used to perform basic retrieval and update operations.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Stream
    {
        #region Private Members and Accessors
        #region Member Variables
        public const string FLAG_TEMPORAL = "temporal";
        public const string FLAG_SPATIAL = "spatial";
        private IAPIClient _Client;
        private string _ID;
        private string _URL;
        private string _Name;
        private string _Flags;
        private string _DataUrl;
        private string _CommandsUrl;
	    private DateTime _CreatedAt;
	    private DateTime _LastUpdated;
        private List<DataSource> _DataSources;
        #endregion Member Variables

        #region Accessors
        [JsonIgnore]
        public IAPIClient StreamAPIClient
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
        public string flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dataUrl
        {
            get { return _DataUrl; }
            set { _DataUrl = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string commandsUrl
        {
            get { return _CommandsUrl; }
            set { _CommandsUrl = value; }
        }

        [JsonIgnore]
        public DateTime createdAt
        {
            get { return _CreatedAt; }
            set { _CreatedAt = value; }
        }

        [JsonProperty]
        public DateTime lastUpdated
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }

        [JsonProperty]
        public List<DataSource> dataSources
        {
            get
            {
                if (_DataSources == null)
                    _DataSources = new List<DataSource>();
                return _DataSources;
            }
            set { _DataSources = value; }
        }
        #endregion Accessors
        #endregion Private Members and Accessors

        #region Stream Methods
        /// <summary>        
        /// Create a Stream locally from a JSON Object
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="JSONObject"></param>
        /// <returns></returns>
        public static Stream CreateStream(IAPIClient Client, JObject JSONObject)
        {
            try
            {
                //assume a single stream is being passed in
                Stream NewStream = JsonConvert.DeserializeObject<Stream>(JSONObject["stream"].ToString());
                NewStream.StreamAPIClient = Client;
                return NewStream;
            }
            catch (Exception ex)
            {
                throw new APIException("Could not deserialize the JSON", ex);
            }
        }

        /// <summary>        
        /// Serialize the project object, wrapping it in a "stream" group
        /// </summary>
        /// <returns></returns>
        public JObject ToJSON()
        {
            try
            {
                JObject JSONObject = new JObject();
                JSONObject.Add("stream", JToken.FromObject(this));
                return JSONObject;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to serialize the JSON", ex);
            } 
        }
        #endregion Stream Methods

        #region AppendData Methods
        /// <summary>        
        /// Append data records to an existing Stream
        /// </summary>
        /// <param name="data"></param>
        public void AppendData(List<String[]> data)
        {
            StreamAPIClient.AppendData(this.dataUrl, data);
        }

        /// <summary>        
        /// Append data records to an existing Stream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void AppendData(List<String[]> data, UploadCallback callback)
        {
            StreamAPIClient.AppendData(this.dataUrl, data, callback);
        }

        /// <summary>        
        /// Append data records to an existing Stream
        /// </summary>
        /// <param name="NewData"></param>
        public void AppendData(DataTable NewData)
        {
            AppendData(NewData.Data);
        }

        /// <summary>        
        /// Append data records to an existing Stream
        /// </summary>
        /// <param name="NewData"></param>
        /// <param name="callback"></param>
        public void AppendData(DataTable NewData, UploadCallback callback)
        {
            AppendData(NewData.Data, callback);
        }
        #endregion AppendData Methods
    }
}
