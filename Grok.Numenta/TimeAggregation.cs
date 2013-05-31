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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>    
    /// The TimeAggregation class represents the Aggregation used in a given Model.
    /// </summary>
    public class TimeAggregation
    {
        #region Members and Accessors
        #region Members
        /**
	     * Aggregates input data into buckets of one second intervals.
	     */
	    public const string INTERVAL_SECONDS = "seconds";
	
	    /**
	     * Aggregates input data into buckets of one minute intervals.
	     */
	    public const string INTERVAL_MINUTES = "minutes";
	
	    /**
	     * Aggregates input data into buckets of one hour intervals.
	     */
	    public const string INTERVAL_HOURS = "hours";

	    /**
	     * Aggregates input data into buckets of one day intervals.
	     */
	    public const string INTERVAL_DAYS = "days";
	
	    /**
	     * Aggregates input data into buckets of one week intervals.
	     */
	    public const string INTERVAL_WEEKS = "weeks";
	
	    /**
	     * Aggregates input data into buckets of one month intervals.
	     */
	    public const string INTERVAL_MONTHS = "months";
	
	    /**
	     * Aggregates input data using the sum of all values during
	     * the interval.  This function can only be used with
	     * <code>SCALAR</code> fields.
	     */
	    public const string FUNCTION_SUM = "sum";
	
	    /**
	     * Aggregates input data using the average/mean of all values
	     * during the interval.  This function can only be used with
	     * <code>SCALAR</code> fields.
	     */
	    public const string FUNCTION_MEAN = "mean";
	
	    /**
	     * Aggregates input data using the first value during the interval.
	     */
	    public const string FUNCTION_FIRST = "first";
	
	    /**
	     * Aggregates input data using the last value during the interval.
	     */
	    public const string FUNCTION_LAST = "last";
	
	    /**
	     * Aggregates input data using the minimum value during the interval.
	     */
	    public const string FUNCTION_MIN = "min";
	
	    /**
	     * Aggregates input data using the maximum value during the interval.
	     */
        public const string FUNCTION_MAX = "max";

        private Dictionary<string, int> _Interval;
        private List<string[]> _FieldOverrides;
        #endregion

        #region Accessors
        /// <summary>        
        /// Returns the number of seconds in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetSeconds()
        {
            return GetUnits(INTERVAL_SECONDS);
        }

        /// <summary>        
        /// Sets the number of seconds in the aggregation interval.
        /// </summary>
        /// <param name="seconds"></param>
        public void SetSeconds(int seconds)
        {
            SetUnits(INTERVAL_SECONDS, seconds);
        }

        /// <summary>        
        /// Returns the number of minutes in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetMinutes()
        {
            return GetUnits(INTERVAL_MINUTES);
        }

        /// <summary>        
        /// Sets the number of minutes in the aggregation interval.
        /// </summary>
        /// <param name="minutes"></param>
        public void SetMinutes(int minutes)
        {
            SetUnits(INTERVAL_MINUTES, minutes);
        }

        /// <summary>        
        /// Returns the number of hours in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetHours()
        {
            return GetUnits(INTERVAL_HOURS);
        }

        /// <summary>        
        /// Sets the number of hours in the aggregation interval.
        /// </summary>
        /// <param name="hours"></param>
        public void SetHours(int hours)
        {
            SetUnits(INTERVAL_HOURS, hours);
        }

        /// <summary>        
        /// Returns the number of days in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetDays()
        {
            return GetUnits(INTERVAL_DAYS);
        }

        /// <summary>        
        /// Sets the number of days in the aggregation interval.
        /// </summary>
        /// <param name="days"></param>
        public void SetDays(int days)
        {
            SetUnits(INTERVAL_DAYS, days);
        }

        /// <summary>        
        /// Returns the number of weeks in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetWeeks()
        {
            return GetUnits(INTERVAL_WEEKS);
        }

        /// <summary>        
        /// Returns the number of weeks in the aggregation interval.
        /// </summary>
        /// <param name="weeks"></param>
        public void SetWeeks(int weeks)
        {
            SetUnits(INTERVAL_WEEKS, weeks);
        }

        /// <summary>        
        /// Returns the number of months in the aggregation interval.
        /// </summary>
        /// <returns></returns>
        public int GetMonths()
        {
            return GetUnits(INTERVAL_MONTHS);
        }

        /// <summary>        
        /// Sets the number of months in the aggregation interval.
        /// </summary>
        /// <param name="months"></param>
        public void SetMonths(int months)
        {
            SetUnits(INTERVAL_MONTHS, months);
        }

        /// <summary>        
        /// Private method to return a specific unit of aggregation from the dictionary
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private int GetUnits(string Name)
        {
            return interval[Name];
        }

        /// <summary>        
        /// Private method to set a specific unit of aggregation from the dictionary
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        private void SetUnits(string Name, int Value)
        {
            if (Value <= 0)
            {
                if (interval.ContainsKey(Name))
                {
                    interval.Remove(Name);
                }
            }
            else
            {
                interval.Add(Name, Value);
            }
        }

        /// <summary>        
        /// Accessor methods to set and retrieve the aggregation interval
        /// </summary>
        [JsonProperty]
        public Dictionary<string, int> interval
        {
            get 
            {
                if (_Interval == null)
                    _Interval = new Dictionary<string, int>();
                return _Interval; 
            }
            set { _Interval = value; }
        }

        /// <summary>        
        /// Accessor method to set and retrieve the aggregation field overrides
        /// </summary>
        [JsonProperty("fields")]
        public List<string[]> FieldOverrides
        {
            get
            {
                if (_FieldOverrides == null)
                    _FieldOverrides = new List<string[]>();
                return _FieldOverrides;
            }
            set { _FieldOverrides = value; }
        }
        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>        
        /// Default constructor
        /// </summary>
        public TimeAggregation() { }

	    /// <summary>        
        /// Constructor that takes an Interval string (e.g.: hours) and creates a new window of a single unit.
	    /// </summary>
	    /// <param name="Interval"></param>
	    public TimeAggregation(string Interval) 
        {
            this.interval.Add(Interval, 1);
	    }

	    /// <summary>        
        /// Constructor that takes a JSON representation (i.e.: from the REST API) and creates a code representation of the
        /// aggregation settings.
	    /// </summary>
	    /// <param name="JSONObject"></param>
	    public TimeAggregation(JObject JSONObject) 
        {
		    this.interval = new Dictionary<string, int>();
		    try 
            {
                if (((JProperty)JSONObject.First).Name.Equals("aggregation"))
                    JSONObject = JSONObject["aggregation"].ToObject<JObject>();

                JToken IntervalJSON = JSONObject["interval"];

                if (IntervalJSON != null)
                    this.interval[((JProperty)IntervalJSON.First).Name] = ((JProperty)IntervalJSON.First).Value.ToObject<int>();

                JToken fieldsJson = JSONObject["fields"];

			    if (fieldsJson != null) 
                {
				    for (int i = 0; i < fieldsJson.Children().Count(); i++) 
                    {
                        JArray fieldJson = fieldsJson[i].ToObject<JArray>();
                        FieldOverrides.Add(new string[] { (string)fieldJson[0], (string)fieldJson[1] });
				    }
			    }
			
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Failed to parse the JSON into a Time Aggregation Object", ex);
		    }
	    }
        #endregion Constructors

        #region Helper Methods
        /// <summary>        
        /// Returns a JSON representation of the Time Aggregation
        /// </summary>
        /// <returns>A JSON Representation of the Time Aggregation that can be fed into the Grok REST API</returns>
	    public JObject ToJSON() 
        {
            try
            {
                JObject JSONObject = new JObject();

                JSONObject.Add("aggregation", JToken.FromObject(this));

                return JSONObject;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to serialize the JSON", ex);
            }
        }
        #endregion Helper Methods
    }
}
