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
using Newtonsoft.Json.Linq;


namespace Grok.Numenta
{
    /// <summary>    
    /// Represents a single field in the DataSources of a Stream.
    /// </summary>
    public class DataSourceField
    {
        #region Members and Accessors
        #region Members
        public const string TYPE_SCALAR = "SCALAR";
	    public const string TYPE_DATETIME = "DATETIME";
	    public const string TYPE_CATEGORY = "CATEGORY";
	    public const string FLAG_NONE = null;
	    public const string FLAG_TIMESTAMP = "TIMESTAMP";
	    public const string FLAG_LOCATION = "LOCATION";
	    private string _Name;
        private Dictionary<string, string> _DataFormat;
	    private string _Flag;
	    private Object _Max = null;
        private Object _Min = null;
        #endregion Members

        #region Accessors
        [JsonProperty]
        public string name 
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [JsonIgnore]
        public string dataType
        {
            get { return dataFormat["dataType"]; }
            set { dataFormat["dataType"] = value; }
        }

        [JsonIgnore]
        public string formatString
        {
            get { return dataFormat["formatString"]; }
            set { dataFormat["formatString"] = value; }
        }

        [JsonProperty]
        public Dictionary<string, string> dataFormat
        {
            get
            {
                if (_DataFormat == null)
                    _DataFormat = new Dictionary<string, string>();
                return _DataFormat;
            }
            set { _DataFormat = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string flag
        {
            get { return _Flag; }
            set { _Flag = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object max
        {
            get 
            {
                if (_Max == null)
                    return _Max;
                else
                    return (double)_Max; 
            }
            set { _Max = Convert.ToDouble(value); }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object min
        {
            get
            {
                if (_Min == null)
                    return _Min;
                else
                    return (double)_Min;
            }
            set { _Min = Convert.ToDouble(value); }
        }
        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>        
        /// Default Constructor
        /// </summary>
	    public DataSourceField() { }
	
        /// <summary>        
        /// Constructor for a new SCALAR field
        /// </summary>
        /// <param name="Name"></param>
	    public DataSourceField(string Name) : this(Name, DataSourceField.TYPE_SCALAR) { }
	
        /// <summary>        
        /// Constructor for a new field
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="DataType"></param>
	    public DataSourceField(string Name, string DataType) : this(Name, DataType, DataSourceField.FLAG_NONE) { } 
	
        /// <summary>        
        /// Constructor for a new field with a Flag
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="DataType"></param>
        /// <param name="flag"></param>
	    public DataSourceField(string Name, string DataType, string flag) 
        {
		    this.name = Name;
		    this.dataType = DataType;
		    this.flag = flag;
	    }
	
        /// <summary>        
        /// Constructor for a new SCALAR field with MIN and MAX values
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
	    public DataSourceField(string Name, double Min, double Max) 
        {
		    this.name = Name;
		    this.dataType = TYPE_SCALAR;
		    this.flag = FLAG_NONE;
		    this.min = Min;
		    this.max = Max;
	    }
	
        /// <summary>        
        /// Constructor for a new DataSourceField from a JSON Object (e.g.: from the API)
        /// </summary>
        /// <param name="JSONObject"></param>
	    public DataSourceField(JObject JSONObject) 
        {
		    try 
            {
                this.name = JSONObject["name"].ToString();
                if (JSONObject["flag"] != null)
			        this.flag = JSONObject["flag"].ToString();
			
			    JObject formatJson = JSONObject["dataFormat"].ToObject<JObject>();
                if (formatJson["dataType"] != null)
                    this.dataType = formatJson["dataType"].ToString();
                if (formatJson["formatString"] != null)
                    this.formatString = formatJson["formatString"].ToString();
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Failed to deserialize JSON", ex);
		    }
	    }
        #endregion Constructors

        #region Helper Methods
        /// <summary>        
        /// Returns a JSON representation of a DataSourceField for use in the API
        /// </summary>
        /// <returns></returns>
        public JObject ToJSON()
        {
            return JToken.FromObject(this).ToObject<JObject>();
        }
        #endregion
    }
}
