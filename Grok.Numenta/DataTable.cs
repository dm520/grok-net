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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvHelper;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 20 November 2012
    /// Class: DataTable
    /// Description: Represents a Grok DataTable as returned in the /data feeds
    /// </summary>
    public class DataTable
    {
        #region Members and Accessors
        #region Members
        private List<string> _Names;
	    private List<string[]> _Data;
        #endregion Members

        #region Accessors
        public List<string> Names
        {
            get 
            {
                if (_Names == null)
                    _Names = new List<string>();
                return _Names; 
            }
            set { _Names = value; }
        }

        public List<string[]> Data
        {
            get
            {
                if (_Data == null)
                    _Data = new List<string[]>();
                return _Data;
            }
            set { _Data = value; }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 05 December 2012
        /// Method: [string Key]
        /// Description: Returns a List representing a data column for the index of a specific key value
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public List<string> this[string Key]
        {
            get
            {
                return this[Names.IndexOf(Key)];
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 05 December 2012
        /// Method: [int Index]
        /// Description: Returns a List representing a data column for the index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public List<string> this[int Index]
        {
            get
            {
                List<string> DataColumn = new List<string>();

                //if not found, throw an exception
                if (Index == -1 || Data == null || Data[0].Length - 1 < Index)
                    throw new KeyNotFoundException();
                else
                {
                    foreach (string[] sArray in Data)
                    {
                        DataColumn.Add(sArray[Index]);
                    }
                }
                return DataColumn;
            }
        }

        /// <summary>
        /// Contains the index of the field representing time for this DataTable. 
        /// </summary>
        public int TimestampIndex { get; set; }

        /// <summary>
        /// Contains the index of the field representing the Predicted Field Prediction value for this DataTable
        /// </summary>
        public int PredictionFieldIndex { get; set; }

        /// <summary>
        /// Contains the index of the field representing the Predicted Field Actual value for this DataTable
        /// </summary>
        public int PredictedFieldIndex { get; set; }

        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Constructor that takes a JSON Object representing a DataTable
        /// </summary>
        /// <param name="JSONObject"></param>
	    public DataTable(JObject JSONObject) 
        {
		    try 
            {
                string key = string.Empty;
                if (((JProperty)JSONObject.First).Name.Equals("output"))
                    key = "output";
                else if (((JProperty)JSONObject.First).Name.Equals("input"))
                    key = "input";

                JArray DataNamesJSONArray = JSONObject[key]["names"].ToObject<JArray>();
                foreach (JValue value in DataNamesJSONArray)
                    this.Names.Add(value.ToString());

                JArray DataJSONArray = JSONObject[key]["data"].ToObject<JArray>();
                foreach (JArray CurrentRow in DataJSONArray)
                {
                    string[] values = new string[CurrentRow.Count];
                    int index = 0;
                    foreach (Object value in CurrentRow)
                    {
                        values[index] = value.ToString();
                        index++;
                    }

                    this.Data.Add(values);
			    }

                // Get meta data
                if (JSONObject[key]["meta"] != null)
                {
                    JObject Meta = JSONObject[key]["meta"].ToObject<JObject>();
                    if (Meta != null)
                    {
                        if (Meta["timestampIndex"] != null)
                        {
                            TimestampIndex = Meta["timestampIndex"].Value<int>();
                        }
                        if (Meta["predictedFieldIndex"] != null)
                        {
                            PredictedFieldIndex = Meta["predictedFieldIndex"].Value<int>();
                        }
                        if (Meta["predictedFieldPredictionIndex"] != null)
                        {
                            PredictionFieldIndex = Meta["predictedFieldPredictionIndex"].Value<int>();
                        }
                    }
                }
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Failed to serialize the JSON", ex);
		    }
	    }
	
	    /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Creates a new DataTable based on a CSV File
	    /// </summary>
	    /// <param name="InputFile"></param>
        public DataTable(FileInfo InputFile) : this(InputFile, false) { }
	
	    /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Creates a new DataTable based on a CSV File
	    /// </summary>
	    /// <param name="InputFile"></param>
	    /// <param name="HasHeaders"></param>
        public DataTable(FileInfo InputFile, bool HasHeaders) : this(InputFile, HasHeaders, int.MaxValue) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="InputFile"></param>
        /// <param name="HasHeaders"></param>
        /// <param name="MaxRows"></param>
        public DataTable(FileInfo InputFile, bool HasHeaders, int MaxRows) : this(InputFile, HasHeaders, MaxRows, 0) { }
	    /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 December 2012
        /// Method: DataTable
        /// Description: Creates a new DataTable based on a CSV File
	    /// </summary>
	    /// <param name="InputFile"></param>
	    /// <param name="HasHeaders"></param>
	    /// <param name="MaxRows"></param>
        /// <param name="SkipRows"></param>
        public DataTable(FileInfo InputFile, bool HasHeaders, int MaxRows, int SkipRows)
        {
		    CsvReader Reader = null;
		
		    try 
            {
			    this.Data = new List<string[]>();

                Reader = new CsvReader(new StreamReader(InputFile.FullName));
                Reader.Read();
		    
		        if (HasHeaders) 
                {
                    this.Names = Reader.FieldHeaders.ToList();
		        } 
                else 
                {
		    	    this.Names = null;
                    Data.Add(Reader.FieldHeaders);
		        }
                for (int i = 0; i < SkipRows; i++)
                    Reader.Read();

                while (Reader.Read() && Data.Count < MaxRows)
                    Data.Add(Reader.CurrentRecord);
		    } 
            catch (IOException ex) 
            {
			    throw new APIException("Failed to read from the CSV File: " + InputFile.FullName, ex);
			
		    } 
            finally 
            {
			    if (Reader != null) 
                {
				    try 
                    {
					    Reader.Dispose();
				    } 
                    catch
                    {
                        //do nothing
				    }
			    }
		    }
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Creates a new DataTable based on a 2-D string array
        /// </summary>
        /// <param name="InputData"></param>
	    public DataTable(string[][] InputData) 
        {
		    this.Names = null;
            this.Data = InputData.OfType<string[]>().ToList();
	    }
	    
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Creates an empty data table
        /// </summary>
	    public DataTable() 
        {
		    this.Names = null;
		    this.Data = new List<string[]>();
	    }
        #endregion Constructors

        #region Helper Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Gets the last row's ID.
        /// Note: This assumes the Grok output format, where the first column is an integer identifier.
        /// </summary>
        /// <returns></returns>
	    public int GetLastRowId() 
        {
		    int result = 0;
		    if (Data.Count > 0) {
			    try 
                {
                    result = Int32.Parse(Data.Last()[0]);
			    } 
                catch
                {
                    //do nothing
			    }
		    }
		    return result;
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November 2012
        /// Method: DataTable
        /// Description: Writes the data table out to the specified file in CSV format.
        /// </summary>
        /// <param name="OutputFile"></param>
	    public void WriteCSV(FileInfo OutputFile) 
        {
		    CsvWriter Writer = null;
		    try 
            {
                Writer = new CsvWriter(new StreamWriter(OutputFile.FullName));
                foreach (string value in Names)
                    Writer.WriteField(value);
                Writer.NextRecord();

                foreach (string[] row in Data)
                {
                    foreach (string value in row)
                        Writer.WriteField(value.Replace("\n",""));
                    Writer.NextRecord();
                }
			
		    } 
            catch (IOException ex) 
            {
			    throw new APIException("Failed to write to the CSV File: " + OutputFile.FullName, ex);
			
		    } 
            finally 
            {
                if (Writer != null) 
                {
				    try 
                    {
                        Writer.Dispose();					
				    } 
                    catch
                    {
                        //do nothing
				    }
			    }
		    }
        }

        #endregion Helper Methods
    }
}
