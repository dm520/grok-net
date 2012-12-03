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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 19 November 2012
    /// Class: DataSource
    /// Description: Represents a DataSource in a Stream.
    /// </summary>
    public class DataSource
    {
        #region Members and Accessors
        #region Members
        /**
	     * A "local" data source represents direct input into the stream.
	     * A stream must have one and only one local data source.
	     */
        public const string TYPE_LOCAL = "local";


        /**
         * A "public" data source represents one of the public data providers.
         * A stream can optionally have many data sources.
         */
        public const String TYPE_PUBLIC = "public";

        private IAPIClient _APIClient;
        private string _Name;
        private string _DataSourceType;
        private List<DataSourceField> _DataSourceFields;
        #endregion Members

        #region Accessors
        [JsonIgnore]
        public IAPIClient Client
        {
            get { return _APIClient; }
            set { _APIClient = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dataSourceType
        {
            get { return _DataSourceType; }
            set { _DataSourceType = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DataSourceField> fields
        {
            get 
            {
                if (_DataSourceFields == null)
                    _DataSourceFields = new List<DataSourceField>();
                return _DataSourceFields; 
            }
            set { _DataSourceFields = value; }
        }
        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: DataSource
        /// Description: Default Constructor
        /// </summary>
        public DataSource() { }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: DataSource
        /// Description: Creates a new LOCAL DataSource with the given name
        /// </summary>
        /// <param name="Name"></param>
        public DataSource(string Name) : this(Name, TYPE_LOCAL) { }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: DataSource
        /// Description: Creates a new DataSource with the given name and Type
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strDataSourceType"></param>
        public DataSource(string strName, string strDataSourceType)
        {
            this.name = strName;
            this.dataSourceType = strDataSourceType;
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: DataSource
        /// Description: Creates a new DataSource from a given JSON Object (e.g.: from the API)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="JSONObject"></param>
        public DataSource(IAPIClient client, JObject JSONObject)
        {
            try
            {
                this.Client = client;
                if (((JProperty)JSONObject.First).Name.Equals("dataSources"))
                    JSONObject = JSONObject["dataSources"][0].ToObject<JObject>();
                this.name = JSONObject["name"].ToString();
                this.dataSourceType = JSONObject["dataSourceType"].ToString();
                this.fields = new List<DataSourceField>();

                JArray FieldsJSON = JSONObject["fields"].ToObject<JArray>();

                foreach (JObject value in FieldsJSON)
                {
                    this.fields.Add(new DataSourceField(value));
                }

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to deserialize JSON", ex);
            }
        }
        #endregion Constructors

        #region Helper Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: ToJSON
        /// Description: Serializes the DataSource as JSON for use with the API
        /// </summary>
        /// <returns></returns>
        public JObject ToJSON() 
        {
            return JToken.FromObject(this).ToObject<JObject>();
        }
        #endregion Helper Methods
    }
}