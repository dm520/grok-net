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
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 03 January 2013
    /// Class: Model
    /// Description: Represents a Grok Model
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Model
    {
        #region Members and Accessors
        #region Member Variables
        public const string COMMAND_SWARM = "swarm";
	    
        public const string COMMAND_PROMOTE = "promote";
	    public const string COMMAND_START = "start";
	    public const string COMMAND_STOP = "stop";
        public const string COMMAND_CLONE = "clone";
	    
        public const string COMMAND_DISABLE_LEARNING = "disableLearning";
	    public const string COMMAND_ENABLE_LEARNING = "enableLearning";

        public const string COMMAND_ADD_LABEL = "addLabel";
        public const string COMMAND_REMOVE_LABEL = "removeLabels";
        public const string COMMAND_GET_LABELS = "getLabels";
        public const string COMMAND_SET_ANOMALY_THRESHOLD = "setAutoDetectThreshold";
        public const string COMMAND_GET_ANOMALY_THRESHOLD = "getAutoDetectThreshold";


        public const string TYPE_ANOMALY = "anomalyDetector";
        public const string TYPE_PREDICTOR = "predictor";
	
	    private IAPIClient _Client;
	    private string _ID;
	    private string _URL;
	    private string _Name;
        private string _ModelType;
	    private string _InputStreamId;
	    private string _PredictedField;
	    private string _Status;
	    private string _CommandsUrl;
	    private string _DataUrl;
	    private string _SwarmsUrl;
        private string _CheckpointsURL;
        private string _ProjectID;
        private string _FastSwarmModelID;
	    private DateTime _CreatedAt;
	    private DateTime _LastUpdated;
        private Swarm _LastSwarm;
	    private TimeAggregation _Aggregation;
	    private List<int> _PredictionSteps;
	    private JObject _Parameters;
        private Dictionary<string, Object> _CustomErrorMetric;
        #endregion Member Variables

        #region Accessors
        [JsonIgnore]
        public IAPIClient ModelAPIClient
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
        public string modelType
        {
            get { return _ModelType; }
            set { _ModelType = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string projectId
        {
            get { return _ProjectID; }
            set { _ProjectID = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url
        {
            get { return _URL; }
            set { _URL = value; }
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Swarm lastSwarm
        {
            get { return _LastSwarm; }
            set { _LastSwarm = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string checkpointsUrl
        {
            get { return _CheckpointsURL; }
            set { _CheckpointsURL = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string streamId
        {
            get { return _InputStreamId; }
            set { _InputStreamId = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string predictedField
        {
            get { return _PredictedField; }
            set { _PredictedField = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dataUrl
        {
            get { return _DataUrl; }
            set { _DataUrl = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string swarmsUrl
        {
            get { return _SwarmsUrl; }
            set { _SwarmsUrl = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string commandsUrl
        {
            get { return _CommandsUrl; }
            set { _CommandsUrl = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fastSwarmModelId
        {
            get { return _FastSwarmModelID; }
            set { _FastSwarmModelID = value; }
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

        /// <summary>
        /// NOTE: This needs to be instantiated before setting; if it gets set to an empty value,
        /// then the JSON representation will be invalid, causing an HTTP error when creating
        /// a Model.  Therefore, it needs to be instantiated locally.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> predictionSteps
        {
            get { return _PredictionSteps; }
            set { _PredictionSteps = value; }
        }

        /// <summary>
        /// NOTE: This needs to be instantiated before setting; if it gets set to an empty value,
        /// then the JSON representation will be invalid, causing an HTTP error when creating
        /// a Model.  Therefore, it needs to be instantiated locally.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Object> customErrorMetric
        {
            get { return _CustomErrorMetric; }
            set { _CustomErrorMetric = value; }
        }

        [JsonIgnore]
        public JObject Parameters
        {
            get { return _Parameters; }
            set { _Parameters = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeAggregation aggregation
        {
            get { return _Aggregation; }
            set { _Aggregation = value; }
        }

        public void SetAggregation(string AggregationInterval)
        {
            aggregation = new TimeAggregation(AggregationInterval);
        }
        #endregion Accessors
        #endregion Private Members and Accessors

        #region Constructors
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateModel
        /// Description: Creates a Model from a JSON Object (e.g.: from the API)
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="JSONObject"></param>
        /// <returns></returns>
        public static Model CreateModel(IAPIClient Client, JObject JSONObject)
        {
            try
            {
                //assume a single user is being passed in
                Model NewModel = JsonConvert.DeserializeObject<Model>(JSONObject["model"].ToString());
                NewModel.ModelAPIClient = Client;
                if (NewModel.lastSwarm != null)
                {
                    NewModel.lastSwarm.SwarmAPIClient = Client;
                }
                return NewModel;
            }
            catch (Exception ex)
            {
                throw new APIException("Could not deserialize the JSON", ex);
            }
        }
        /// <summary>
        /// Create a prediction model
        /// </summary>
        public Model(): this(TYPE_PREDICTOR)
        {         
        }
        /// <summary>
        /// Creates a model
        /// </summary>
        /// <param name="type">TYPE_ANOMALY or TYPE_PREDICTOR</param>
        public Model(string type)
        {
            _ModelType = type;
        }
        #endregion Constructors

        #region Model Anomaly Methods
        /// <summary>
        /// This method sets the threshold used for automatic classification. 
        /// If different from current setting, it will recalculate all classifications within the anomaly cache. 
        /// If it is set above 1.0, it will not classify anything.  
        /// </summary>
        /// <param name="threshold">the value to set the automatic classification threshold to</param>
        public void SetAnomalyThreshold(double threshold)
        {
            try
            {
                JObject commandParams = new JObject();
                commandParams.Add("autoDetectThreshold", threshold);
                JObject JSONResponse = ModelAPIClient.SendModelCommand(this.commandsUrl, COMMAND_SET_ANOMALY_THRESHOLD, commandParams);
                if (JSONResponse["error"] != null)
                {
                    throw new APIException(JSONResponse["error"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to Set Anomaly Threshold: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// This method returns the threshold used for automatic classification
        /// </summary>
        /// <returns></returns>
        public double GetAnomalyThreshold()
        {
            try
            {
                JObject response = ModelAPIClient.SendModelCommand(this, COMMAND_GET_ANOMALY_THRESHOLD);
                if (response["error"] != null)
                {
                    throw new APIException(response["error"].ToString());
                }
                if (response["autoDetectThreshold"] != null)
                {
                    return response["autoDetectThreshold"].Value<double>();
                }
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to Get Anomaly Threshold: " + ex.Message, ex);
            }
            return Double.NaN;
        }
        /// <summary>
        /// This method adds the label ‘labelName’ to each point within the range [startRecord, endRecord].
        /// </summary>
        /// <param name="startRecord"></param>
        /// <param name="endRecord"></param>
        /// <param name="labelName"></param>
        public void AddLabel(int startRecord, int endRecord, string labelName)
        {
            try
            {
                Dictionary<string, string> commandParams = new Dictionary<string, string>();
                commandParams.Add("startRecordID", startRecord.ToString());
                commandParams.Add("endRecordID", endRecord.ToString());
                commandParams.Add("labelName", labelName);
                JObject JSONResponse = ModelAPIClient.SendModelCommand(this, COMMAND_ADD_LABEL, commandParams);
                if (JSONResponse["error"] != null)
                {
                    throw new APIException(JSONResponse["error"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to add label: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// This method returns the labels of a range of records. 
        /// </summary>
        /// <param name="startRecord"></param>
        /// <param name="endRecord"></param>
        /// <returns>
        /// A dictionary mapping each record to a list of labels associated with that record
        /// or an empty dictionary if the model is still re-processing the newly added labels.
        /// </returns>
        public Dictionary<int, List<string>> GetLabels(int startRecord = 0, int endRecord = -1)
        {
            try
            {
                Dictionary<string, string> commandParams = new Dictionary<string, string>();
                if (startRecord > 0)
                    commandParams.Add("startRecordID", startRecord.ToString());
                if (endRecord > 0)
                    commandParams.Add("endRecordID", endRecord.ToString());
                JObject response = ModelAPIClient.SendModelCommand(this, COMMAND_GET_LABELS, commandParams);
                if (response["error"] != null)
                {
                    // Check if the model is still processing
                    if (response["isProcessing"] != null && response["isProcessing"].Value<Boolean>())
                    {
                        // Nothing to return here
                        /// FIXME: Should we block and wait for the labels instead?
                        return new Dictionary<int, List<string>>(); ;
                    }
                    throw new APIException(response["error"].ToString());
                }

                // build dictionary from response
                Dictionary<int, List<string>> results = new Dictionary<int, List<string>>();
                foreach (var row in response["recordLabels"])
                {
                    var id = row["ROWID"].Value<int>();
                    List<string> list = new List<string>();
                    foreach (var label in row["labels"])
                    {
                        list.Add((string)label);
                    }
                    results.Add(id, list);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to get labels: " + ex.Message, ex);
            }
        }
        /// <summary>
        /// This method removes labels from a range of records
        /// </summary>
        /// <param name="startRecord"></param>
        /// <param name="endRecord"></param>
        /// <param name="labelFilter"></param>
        public void RemoveLabels(int startRecord = 0, int endRecord = -1, string labelFilter = null)
        {
            try
            {
                Dictionary<string, string> commandParams = new Dictionary<string, string>();
                commandParams.Add("startRecordID", startRecord.ToString());
                if (endRecord != -1)
                    commandParams.Add("endRecordID", endRecord.ToString());
                if (labelFilter != null)
                    commandParams.Add("labelFilter", labelFilter);
                JObject JSONResponse = ModelAPIClient.SendModelCommand(this, COMMAND_REMOVE_LABEL, commandParams);
                if (JSONResponse["error"] != null)
                {
                    throw new APIException((string)JSONResponse["error"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to remove labels: " + ex.Message, ex);
            }
        }
        
        #endregion
        #region Model Swarm Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateSwarm
        /// Description: Creates a Swarm for the Model
        /// </summary>
        /// <returns></returns>
        public Swarm CreateSwarm()
        {
            return ModelAPIClient.CreateSwarm(this.swarmsUrl);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateSwarm
        /// Description: Sends a command to the API to start swarming the model and return the given Swarm
	    /// </summary>
	    /// <param name="ParamsToSwarmOn"></param>
	    /// <returns></returns>
	    public Swarm SwarmModel(JObject ParamsToSwarmOn) 
        {
		    try 
            {
                JObject JSONResponse = ModelAPIClient.SendModelCommand(this.commandsUrl, COMMAND_SWARM, ParamsToSwarmOn);
                return Swarm.CreateSwarm(ModelAPIClient, JSONResponse);		
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Failed to swarm model", ex);
		    }
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateSwarm
        /// Description: Sends a command to the API to start swarming the model and return the given Swarm
	    /// </summary>
        /// <param name="ParamsToSwarmOn"></param>
	    /// <returns></returns>
	    public Swarm SwarmModel(Dictionary<string, string> ParamsToSwarmOn) 
        {
		    try 
            {
			    JObject JSONResponse = ModelAPIClient.SendModelCommand(this, COMMAND_SWARM, ParamsToSwarmOn);
                return Swarm.CreateSwarm(ModelAPIClient, JSONResponse);
		
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Failed to swarm model", ex);
		    }
	    }
	
	    /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateSwarm
        /// Description: Sends a command to the API to start swarming the model and return the given Swarm
	    /// </summary>
	    /// <param name="SwarmSize"></param>
	    /// <returns></returns>
	    public Swarm SwarmModel(string SwarmSize) 
        {
		    Dictionary<string, string> Parameters = new Dictionary<string, string>();
		    Parameters.Add("size", SwarmSize);
		    return SwarmModel(Parameters);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: CreateSwarm
        /// Description: Sends a command to the API to start swarming the model and return the given Swarm
        /// </summary>
        /// <returns></returns>
	    public Swarm SwarmModel() 
        {
		    Dictionary<string, string> Parameters = new Dictionary<string, string>();
		    return SwarmModel(Parameters);
	    }
        #endregion Model Swarm Methods

        #region Model Promotion Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: Promote
        /// Description: Sends a command to the API to Promote the Model
        /// </summary>
	    public void Promote() 
        {
		    Promote(false);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: Promote
        /// Description: Sends a command to the API to Promote the Model
        /// </summary>
        /// <param name="verbose"></param>
	    public void Promote(bool verbose) 
        {
		    DataTable OriginalData = RetrieveData();
		    StartPromote();
            WaitForStatus("running", verbose);
		    WaitForRowId(OriginalData.Data.Count, verbose);
		    WaitForStabilization(verbose);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: Promote
        /// Description: Sends a command to the API to Promote the Model
        /// </summary>
	    public void StartPromote() 
        {
		    ModelAPIClient.SendModelCommand(this, COMMAND_PROMOTE);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: Start
        /// Description: Sends a command to the API to Start the Model
        /// </summary>
        public void Start()
        {
            ModelAPIClient.SendModelCommand(this, COMMAND_START);
        }
        /// <summary>
        /// Sends a command to Stop the model
        /// </summary>
        public void Stop()
        {
            ModelAPIClient.SendModelCommand(this, COMMAND_STOP);
        }
        /// <summary>
        /// Clone model based on the last checkpoint
        /// </summary>
        /// <returns>Cloned Model</returns>
        public Model Clone()
        {
            return ModelAPIClient.CloneModel(this);
        }
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: DisableLearning
        /// Description: Sends a command to the API to Disable Learning in the Model
        /// </summary>
        public void DisableLearning()
        {
            ModelAPIClient.SendModelCommand(this, COMMAND_DISABLE_LEARNING);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: EnableLearning
        /// Description: Sends a command to the API to enable learning in the Model
        /// </summary>
        public void EnableLearning()
        {
            ModelAPIClient.SendModelCommand(this, COMMAND_ENABLE_LEARNING);
        }
        #endregion Model Promotion Methods

        #region Stabilization Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: WaitForStatus
        /// Description: Pauses execution until the Model reaches a given status
        /// </summary>
        /// <param name="Status"></param>
	    public void WaitForStatus(string Status) 
        {
		    WaitForStatus(Status, false);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: WaitForStatus
        /// Description: Pauses execution until the Model reaches a given status
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="Verbose"></param>
	    public void WaitForStatus(string Status, bool Verbose) 
        {
		    Model CurrentModel = ModelAPIClient.RetrieveModel(this.url);
		    while (!CurrentModel.status.Equals("running")) 
            {
			    if (Verbose) 
				    Console.WriteLine("Model not yet running...");

			    Thread.Sleep(1000);
			    CurrentModel = ModelAPIClient.RetrieveModel(this.url);
		    }
		    if (Verbose) 
			    Console.WriteLine("Model running!");
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: WaitForRowId
        /// Description: Pauses execution until the Model reaches a given Row ID
        /// </summary>
        /// <param name="RowId"></param>
	    public void WaitForRowId(int RowId) 
        {
		    WaitForRowId(RowId, false);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: WaitForRowId
        /// Description: Pauses execution until the Model reaches a given Row ID
        /// </summary>
        /// <param name="RowId"></param>
        /// <param name="Verbose"></param>
	    public void WaitForRowId(int RowId, bool Verbose) 
        {
		    DataTable CurrentData = RetrieveData();
            while (CurrentData.Data.Count < RowId)
            {
                if (Verbose)
                    Console.WriteLine("Model catching up (" + CurrentData.Data.Count + " / " + RowId + ")...");
			    Thread.Sleep(1000);
                CurrentData = RetrieveData();
		    }
		    if (Verbose) 
			    Console.WriteLine("Model caught up!");
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: WaitForStabilization
        /// Description: Pauses execution until the Model fully stabilizes
        /// </summary>
	    public void WaitForStabilization() {
		    WaitForStabilization(false);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 03 January 2013
        /// Method: WaitForStabilization
        /// Description: Pauses execution until the Model fully stabilizes
        /// </summary>
        /// <param name="Verbose"></param>
	    public void WaitForStabilization(bool Verbose)
        {
            int LastID = 0;
            DataTable CurrentData = RetrieveData(1, false);
            try
            {
                LastID = Convert.ToInt32(CurrentData["ROWID"][0]);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("Could not retrieve data for this model. Trying again");
                Thread.Sleep(5000);
                try
                {
                    CurrentData = RetrieveData(1, false);
                    LastID = Convert.ToInt32(CurrentData["ROWID"][0]);
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine("Could not retrieve data for this model. Moving on");
                    return;
                }
            }

            while (true)
            {
                Thread.Sleep(5000); 
                CurrentData = RetrieveData(1);
                int CurrentID = Convert.ToInt32(CurrentData["ROWID"][0]);
                if (CurrentID == LastID)
                    break;
                else
                    LastID = CurrentID;
                
                if (Verbose)
                    Console.WriteLine("Model stabilizing (" + (LastID + 1) + " rows)...");
		    }
            if (Verbose)
            {
                Console.WriteLine("Model stabilized at row " + (LastID + 1) + "!");
		    }
	    }
        #endregion Stabilization Methods

        #region RetrieveData Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveData
        /// Description: Retrieves the data from the Model
        /// </summary>
        /// <returns></returns>
	    public DataTable RetrieveData() 
        {
		    return ModelAPIClient.RetrieveOutputData(this.dataUrl);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveData
        /// Description: Retrieves the data from the Model, limiting the ouptut to a specified number of rows
        /// </summary>
        /// <param name="Limit"></param>
        /// <returns></returns>
	    public DataTable RetrieveData(int Limit) 
        {
		    return RetrieveData(-1, Limit, false);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveData
        /// Description: Retrieves the data from the Model, shifting the output to have the predictions line up with the Actual values
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
	    public DataTable RetrieveData(bool Shift) 
        {
		    return RetrieveData(-1, -1, Shift);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveData
        /// Description: Retrieves the data from the Model, limiting the output and shifting the predictions to line up with the actuals
        /// </summary>
        /// <param name="Limit"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
	    public DataTable RetrieveData(int Limit, bool Shift) 
        {
		    return RetrieveData(-1, Limit, Shift);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: RetrieveData
        /// Description: Retrieves the data from the Model, limiting the output and shifting the predictions to line up with the actuals, and starting from a specific Row ID (Offset)
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Limit"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
	    public DataTable RetrieveData(int Offset, int Limit, bool Shift) 
        {
		    return ModelAPIClient.RetrieveOutputData(BuildDataUrl(Offset, Limit, Shift));
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: BuildDataUrl
        /// Description: Builds the Model Data URL from the model URL given the specified inputs
        /// </summary>
        /// <param name="Offset"></param>
        /// <param name="Limit"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
	    private String BuildDataUrl(int Offset, int Limit, bool Shift) 
        {
		    string Parameters = "?";
		
		    if (Offset > 0) 
			    Parameters = "?offset=" + Offset;
		    		
		    if (Limit > 0) 
            {
			    if (Parameters.Length > 1) 
				    Parameters += "&";
			    Parameters += "limit=" + Limit;
		    }
		
		    if (Shift) 
            {
			    if (Parameters.Length > 1) 
				    Parameters += "&";
			    Parameters += "shift=" + Shift;
		    }
		
		    return Parameters.Length > 1 ? this.dataUrl + Parameters : this.dataUrl;
	    }
        #endregion RetrieveData Methods

        #region Helper Methods
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November 2012
        /// Method: ToJSON
        /// Description: Serialize the Model to JSON for use in the API
        /// </summary>
        /// <returns></returns>
	    public JObject ToJSON()
        {
            try
            {
                JObject JSONObject = new JObject();
                JSONObject.Add("model", JToken.FromObject(this));
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
