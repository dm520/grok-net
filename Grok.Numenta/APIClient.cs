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
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// The APIClient class wraps a connection to the API server and
    /// provides helper methods to invoke the API.
    ///
    /// <h4>About REST</h4>
    ///
    /// <p>Many of the <code>APIClient</code> methods take a <code>url</code>
    /// parameter.  Don't worry!  First of all, there are helper methods that can
    /// bypass using URLs all together.  Second, if you prefer to use advanced
    /// REST functionality, you do not need to hunt down URLs or manually
    /// construct arcane query strings.  The URLs are provided to you be other
    /// API calls.</p>
    ///
    /// <p>For example, to create a model you would invoke <code>CreateModel()</code>,
    /// which takes a <code>url</code> parameter.  If you wish to create a model in a
    /// project, you could use <code>project.modelsUrl</code>.  Or, if you wish
    /// to skip using projects, you could use <code>user.modelsUrl</code>.</p>
    ///
    /// <h4>Example</h4>
    ///
    /// <p>The following example shows how to create a new <code>APIClient</code>
    /// and retrieve the default user account associated with the API key.</p>
    ///
    /// <pre><code>
    /// APIClient client = new APIClient(myApiKey);
    /// User user = client.DefaultUser;
    /// Console.Write("My name is " + user.firstName);
    /// </code></pre>
    /// </summary>
    public class APIClient : IAPIClient
    {
        #region Members and Accessors
        #region Member Data
        private static string _NumentaURI = "https://api.numenta.com/";
        private static string _Version = "v2";
        private static int _MaxRowsPerUpload = 500;

        private string _APIKey = string.Empty;
        private User _DefaultUser;

        private AssemblyName _CurrentAssembly;
        private Version _AssemblyVersion;

        private HttpClient _HttpClient;
 
        #endregion Member Data

        #region Accessors
        /// <summary>
        /// Grok API Key 
        /// </summary>
        public string APIKey
        {
            get { return _APIKey; }
            set { _APIKey = value; }
        }
        /// <summary>
        ///The default user object.
        ///For user account level activities, actions are performed on this account.
        ///It is initially populated with the first user account that the API key has access to.
        ///In the common case, this is the only user account for the API key.
        /// </summary>
        public User DefaultUser
        {
            get
            {
                if (_DefaultUser == null)
                    InitDefaultUser();
                return _DefaultUser;
            }
            set { _DefaultUser = value; }
        }

        public string NumentaURI
        {
            get { return _NumentaURI; }
            set { _NumentaURI = value; }
        }
        /// <summary>
        /// Current API Version
        /// </summary>
        public string VersionNumber
        {
            get
            {
                _CurrentAssembly = this.GetType().Assembly.GetName();
                _AssemblyVersion = _CurrentAssembly.Version;
                return _AssemblyVersion.ToString(4);
            }
        }
        /// <summary>
        /// HTTP Client used to send and receive HTTP requests
        /// </summary>
        public HttpClient HTTPClient
        {
            get
            {
                if (_HttpClient == null)
                    _HttpClient = new HttpClient();
                return _HttpClient;
            }
            set { _HttpClient = value; }
        }
        /// <summary>
        /// Number of times to try to connect to the API server in case of server error (500) 
        /// or connection timeout error before throwing an exception.
        /// Default value: 2
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// The number of seconds to wait between retries.
        /// Default value: 10
        /// </summary>
        public int RetryAter { get; set; }

        /// <summary>
        /// Number of seconds to wait for the API server response.
        /// </summary>
        public int Timeout {
            get
            {
                return (int)HTTPClient.Timeout.TotalSeconds;
            }
            set 
            {
                HTTPClient.Timeout = TimeSpan.FromSeconds(value);
            } 
        }

        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>
        /// Constructor taking the API Key of the User 
        /// </summary>
        /// <param name="UserAPIKey">The API Key to use</param>
        public APIClient(string UserAPIKey) : this(UserAPIKey, _NumentaURI) { }

        /// <summary>
        /// Constructor taking the API Key of the User and a base URL (e.g.: https://api.numenta.com)
        /// </summary>
        /// <param name="UserAPIKey">The API Key to use</param>
        /// <param name="ConnectionURL">The API Server URL to use</param>
        public APIClient(string UserAPIKey, string ConnectionURL)
        {
            try
            {
                NumentaURI = ConnectionURL;
                if (!NumentaURI.EndsWith("/"))
                    NumentaURI += "/";
            }
            catch (UriFormatException ex)
            {
                throw new APIException("Invalid Connection URL", ex);
            }

            APIKey = UserAPIKey;

            string AuthorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(APIKey + ":"));
            HTTPClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthorizationHeader);
            HTTPClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HTTPClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Grok_API_Client.Net", VersionNumber));

            // Default values for Retry and Timeout
            Retry = 2;
            RetryAter = 10;
            Timeout = 30;
        }
        #endregion Constructors

        #region HTTP Calls
        /// <summary>
        /// Makes an HTTP GET Call to the specified URL
        /// </summary>
        /// <param name="URL">Either an absolute or a relative URL</param>
        /// <returns></returns>
        protected virtual HttpResponseMessage Get(string URL)
        {

            if (!URL.ToUpper().StartsWith("HTTP"))
                URL = NumentaURI + URL;

            int retries = Retry;
            bool resendRequest ;
            HttpResponseMessage response = _HttpClient.GetAsync(URL).Result;
            do
            {
                resendRequest = --retries > 0;

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    // Check for Server error and retry the connection
                    if (response.StatusCode >= HttpStatusCode.InternalServerError)
                    {

                        int waitTime = RetryAter;

                        // Get "Retry-After" header
                        RetryConditionHeaderValue retryAfterHeader = response.Headers.RetryAfter;
                        if (retryAfterHeader != null && retryAfterHeader.Delta.HasValue)
                        {
                            waitTime = retryAfterHeader.Delta.Value.Seconds;
                            // Ignore the retry count configuration if server sent "Retry-After" header
                            resendRequest = true;
                        }
                        if (resendRequest) 
                        {
                            // Wait a few seconds before resending the request
                            Thread.Sleep(waitTime * 1000);

                            // Resend the request
                            response = _HttpClient.GetAsync(URL).Result;
                            continue;
                        } 
                    }
                    break; // Any other error throws an exception.
                }
            } while (resendRequest);

           // Throw an exception with the last retry error
           throw new IOException("Failed to connect to " + NumentaURI + " with error " + (int)response.StatusCode + response.ReasonPhrase);
        }

        /// <summary>
        /// Makes an HTTP GET Call to the specified URL to return some JSON
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        protected virtual JObject GetJSONObject(string URL)
        {
            HttpResponseMessage response = Get(URL);
            JObject json = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            if (response.Content.Headers.Expires.HasValue)
            {
                DateTimeOffset expires = response.Content.Headers.Expires.Value;
                TimeSpan offset = response.Content.Headers.Expires.Value.UtcDateTime.Subtract(DateTime.UtcNow);
                json.Add("Expires", (int)Math.Round(offset.TotalSeconds));
            }
            return json;
        }

        /// <summary>
        /// Makes an HTTP POST Call to the specified URL with an attached JSON object
        /// </summary>
        /// <param name="URL">Either an absolute or a relative URL</param>
        /// <param name="Data">JSON Data to post</param>
        /// <returns></returns>
        protected virtual JObject PostJSONObject(string URL, JObject JSONObject)
        {
            return JObject.Parse(Post(URL, JSONObject.ToString()).Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Makes an HTTP POST Call to the specified URL with a payload, which is
        /// assumed to be in a JSON format
        /// </summary>
        /// <param name="URL">Either an absolute or a relative URL</param>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected virtual HttpResponseMessage Post(string URL, string Data)
        {
            StringContent PutContent = new StringContent(Data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _HttpClient.PostAsync(URL, PutContent).Result;

            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                throw new APIException("Failed to connect to " + NumentaURI + " with error " +
                    (int)response.StatusCode + ":" + response.ReasonPhrase + " - " + response.Content.ReadAsStringAsync().Result);
            }
        }

        /// <summary>
        /// Deletes a resource at the specified URL.
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        protected virtual JObject DeleteJSONObject(string URL)
        {
            try
            {
                String result = Delete(URL);
                if (result != null)
                {
                    return JObject.Parse(result);
                }
                return new JObject();
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to delete the object", ex);
            }
        }

        /// <summary>
        /// Deletes the resource at the specified URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected virtual string Delete(string URL)
        {
            try
            {
                Object LockObject = new Object();
                lock (LockObject)
                {
                    HttpResponseMessage response = _HttpClient.DeleteAsync(URL).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return  response.Content != null ?
                                response.Content.ReadAsStringAsync().Result : null;
                    }
                    else
                    {
                        throw new IOException("Failed to connect to " + NumentaURI + " with error " + (int)response.StatusCode + response.ReasonPhrase);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to delete the object", ex);
            }
        }
        #endregion HTTP Calls

        #region User Methods
        /// <summary>
        /// retrieves a list of users
        /// </summary>
        /// <returns></returns>
        public List<User> RetrieveUsers()
        {
            try
            {
                JObject JSONResponse = GetJSONObject(_Version + "/users");

                //deserialize into a list
                List<User> UserList = JsonConvert.DeserializeObject<List<User>>(JSONResponse["users"].ToString());

                //since we're deserializing directly into user objects, we need to ensure that the API Client
                //is set on each user
                foreach (User CurrentUser in UserList)
                    CurrentUser.UserAPIClient = this;

                return UserList;

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to extract the list of users: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Initializes the default user.
        /// Typically, the API key will only have access to one user account.
        /// This method gets that user account.        
        /// </summary>
        public virtual void InitDefaultUser()
        {
            foreach (User user in RetrieveUsers())
            {
                this.DefaultUser = user;
                break;
            }
        }

        /// <summary>
        /// The API expects a JSON Array, so we need to handle that appropriately before serializing
        /// </summary>
        /// <param name="SingleUser"></param>
        /// <returns></returns>
        public User UpdateUser(User SingleUser)
        {
            try
            {
                JObject JSONResponse = PostJSONObject(SingleUser.url, SingleUser.ToJSON());

                return User.CreateUser(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to update the user", ex);
            }
        }
        #endregion User Methods

        #region Project Methods
        /// <summary>
        /// Calls the API to create a new Project 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewProject"></param>
        /// <returns></returns>
        public Project CreateProject(string URL, Project NewProject)
        {
            try
            {
                JObject JSONResponse = PostJSONObject(URL, NewProject.ToJSON());

                return Project.CreateProject(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to create the project", ex);
            }
        }

        /// <summary>        
        ///Calls the API to create a new Project 
        /// </summary>
        /// <param name="NewProject"></param>
        /// <returns></returns>
        public Project CreateProject(Project NewProject)
        {
            return CreateProject(this.DefaultUser.projectsUrl, NewProject);
        }

        /// <summary>
        /// Calls the API to create a new Project 
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public Project CreateProject(string ProjectName)
        {
            Project NewProject = new Project();
            NewProject.name = ProjectName;
            return CreateProject(NewProject);
        }

        /// <summary>
        /// Calls the API to retrieve a list of Projects 
        /// </summary>
        /// <returns></returns>
        public List<Project> RetrieveProjects()
        {
            return RetrieveProjects(this.DefaultUser.projectsUrl);
        }

        /// <summary>
        /// Calls the API to retrieve a list of Projects 
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Project> RetrieveProjects(string URL)
        {
            try
            {
                //get the projects
                JObject JSONResponse = GetJSONObject(URL);

                //deserialize into a list
                List<Project> ProjectList = JsonConvert.DeserializeObject<List<Project>>(JSONResponse["projects"].ToString());

                //since we're just deserializing directly into project objects, we need to ensure that the API Client
                //is set on each project
                foreach (Project CurrentProject in ProjectList)
                    CurrentProject.ProjectAPIClient = this;

                return ProjectList;

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to extract the list of projects", ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a specific Project
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Project RetrieveProject(string URL)
        {
            try
            {
                return Project.CreateProject(this, GetJSONObject(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve the project " + URL, ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a specific Project
        /// </summary>
        /// <param name="CurrentProject"></param>
        /// <returns></returns>
        public Project RetrieveProject(Project CurrentProject)
        {
            return RetrieveProject(CurrentProject.url);
        }

        /// <summary>
        /// Deletes the project at the specified URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public bool DeleteProject(string URL)
        {
            JObject response = DeleteJSONObject(URL);
            if (response["errors"] != null)
            {
                throw new APIException("Error deleting Project");
            }

            return true;
        }

        /// <summary>
        /// Deletes the project 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool DeleteProject(Project project)
        {
            return DeleteProject(project.url);
        }
        #endregion Project Methods

        #region Stream Methods
        /// <summary>
        /// Calls the API to create a Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewStream"></param>
        /// <returns></returns>
        public Stream CreateStream(string URL, Stream NewStream)
        {
            try
            {
                JObject JSONResponse = PostJSONObject(URL, NewStream.ToJSON());

                return Stream.CreateStream(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Stream", e);
            }
        }

        /// <summary>
        /// Calls the API to create a Stream
        /// </summary>
        /// <param name="NewStream"></param>
        /// <returns></returns>
        public Stream CreateStream(Stream NewStream)
        {
            return CreateStream(DefaultUser.streamsUrl, NewStream);
        }

        /// <summary>
        /// Calls the API to retrieve a list of Streams
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Stream> RetrieveStreams(string URL)
        {
            try
            {
                JObject JSONResponse = GetJSONObject(URL);

                //deserialize into a list
                List<Stream> StreamList = JsonConvert.DeserializeObject<List<Stream>>(JSONResponse["streams"].ToString());

                //since we're just deserializing directly into project objects, we need to ensure that the API Client
                //is set on each project
                foreach (Stream CurrentStream in StreamList)
                    CurrentStream.StreamAPIClient = this;

                return StreamList;
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve streams", ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a specific Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Stream RetrieveStream(string URL)
        {
            try
            {
                return Stream.CreateStream(this, GetJSONObject(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve stream " + URL, ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a list of Streams
        /// </summary>
        /// <param name="StreamID"></param>
        /// <returns></returns>
        public Stream RetrieveStreamById(string StreamID)
        {
            String URL = "/" + _Version + "/streams/" + StreamID;
            return RetrieveStream(URL);
        }

        /// <summary>
        /// Calls the API to append data to a Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Data"></param>
        /// <param name="Callback"></param>
        public void AppendData(string URL, List<string[]> Data, UploadCallback Callback)
        {
            try
            {
                int fromIndex = 0;
                int toIndex = Math.Min(_MaxRowsPerUpload, Data.Count);

                if (Callback != null)
                {
                    Callback.OnUpdate(new UploadEvent(0.0));
                }

                while (fromIndex < toIndex)
                {
                    List<JArray> jsonList = new List<JArray>(toIndex - fromIndex);
                    for (int i = fromIndex; i < toIndex; i++)
                    {
                        jsonList.Add(new JArray(Data[i]));
                    }

                    JObject request = new JObject();
                    request.Add("input", new JArray(jsonList));

                    PostJSONObject(URL, request);

                    if (Callback != null)
                    {
                        double percent = ((double)toIndex) / ((double)Data.Count);
                        Callback.OnUpdate(new UploadEvent(percent));
                    }

                    fromIndex = toIndex;
                    toIndex = Math.Min(fromIndex + _MaxRowsPerUpload, Data.Count);
                }

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to Append Data to Stream", ex);
            }
        }

        /// <summary>
        /// Calls the API to append data to a Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Data"></param>
        public void AppendData(string URL, List<string[]> Data)
        {
            AppendData(URL, Data, null);
        }

        /// <summary>
        /// Deletes the Stream at the specified URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public bool DeleteStream(string URL)
        {
            JObject response = DeleteJSONObject(URL);
            if (response["errors"] != null)
            {
                throw new APIException("Error deleting Stream");
            }

            return true;
        }

        /// <summary>
        /// Deletes the Stream 
        /// </summary>
        /// <param name="StreamToDelete"></param>
        /// <returns></returns>
        public bool DeleteStream(Stream StreamToDelete)
        {
            return DeleteStream(StreamToDelete.url);
        }
        #endregion Stream Methods

        #region Swarm Methods
        /// <summary>
        /// Calls the API to retrieve a specific Swarm
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Swarm RetrieveSwarm(string URL)
        {
            try
            {
                return Swarm.CreateSwarm(this, GetJSONObject(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve swarm " + URL, ex);
            }
        }

        /// <summary>
        /// Calls the API to create a Swarm
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Swarm CreateSwarm(string URL)
        {
            try
            {
                JObject JSONResponse = PostJSONObject(URL, new JObject());

                return Swarm.CreateSwarm(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Swarm", e);
            }
        }
        public Swarm CreateSwarm(string URL, JObject parameters)
        {
            try
            {
                JObject options = new JObject();
                options.Add("options", parameters);
                JObject JSONResponse = PostJSONObject(URL, options);
                return Swarm.CreateSwarm(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Swarm", e);
            }
        }
        public Swarm CreateSwarm(string URL, Dictionary<string, string> options)
        {
            try
            {
                JObject JsonParams = null;
                if (options != null && options.Count > 0)
                {
                    JsonParams = new JObject();
                    foreach (string Key in options.Keys)
                    {
                        JsonParams.Add(Key, options[Key]);
                    }
                }
                return CreateSwarm(URL, JsonParams);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Swarm", e);
            }
        }
        /// <summary>
        /// Deletes the Swarm at the specified URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public bool DeleteSwarm(string URL)
        {
            JObject response = DeleteJSONObject(URL);
            if (response["errors"] != null)
            {
                throw new APIException("Error deleting Swarm");
            }

            return true;
        }

        /// <summary>
        /// Deletes the Swarm 
        /// </summary>
        /// <param name="SwarmToDelete"></param>
        /// <returns></returns>
        public bool DeleteSwarm(Swarm SwarmToDelete)
        {
            return DeleteSwarm(SwarmToDelete.url);
        }
        #endregion Swarm Methods

        #region Model Methods
        /// <summary>
        /// Calls the API to send a command for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Command"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public JObject SendModelCommand(string URL, string Command, JObject Parameters)
        {
            try
            {
                JObject request = new JObject();
                request.Add("command", Command);

                if (Parameters != null)
                    request.Add("params", Parameters);

                return PostJSONObject(URL, request);
            }
            catch (Exception ex)
            {
                throw new APIException("Error while sending Model Command " + Command, ex);
            }
        }

        /// <summary>
        /// Calls the API to send a command for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Command"></param>
        /// <param name="Parameterss"></param>
        /// <returns></returns>
        public JObject SendModelCommand(string URL, string Command, Dictionary<string, string> Parameters)
        {
            try
            {
                JObject JsonParams = null;

                if (Parameters != null && Parameters.Count > 0)
                {
                    JsonParams = new JObject();
                    foreach (string Key in Parameters.Keys)
                    {
                        JsonParams.Add(Key, Parameters[Key]);
                    }
                }
                return SendModelCommand(URL, Command, JsonParams);
            }
            catch (Exception ex)
            {
                throw new APIException("Error sending Model Command " + Command, ex);
            }
        }

        /// <summary>
        /// Calls the API to send a command for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public JObject SendModelCommand(string URL, string Command)
        {
            return SendModelCommand(URL, Command, (JObject)null);
        }

        /// <summary>
        /// Calls the API to send a command for a Model
        /// </summary>
        /// <param name="CurrentModel"></param>
        /// <param name="Command"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public JObject SendModelCommand(Model CurrentModel, string Command, Dictionary<string, string> Parameters)
        {
            return SendModelCommand(CurrentModel.commandsUrl, Command, Parameters);
        }

        /// <summary>
        /// Calls the API to send a command for a Model
        /// </summary>
        /// <param name="CurrentModel"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public JObject SendModelCommand(Model CurrentModel, string Command)
        {
            return SendModelCommand(CurrentModel, Command, null);
        }

        /// <summary>
        /// Calls the API to retrieve output data for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public DataTable RetrieveOutputData(string URL)
        {
            try
            {
                return new DataTable(GetJSONObject(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Error retrieving Data Table", ex);
            }
        }

        /// <summary>
        /// Calls the API to create a new Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewModel"></param>
        /// <returns></returns>
        public Model CreateModel(string URL, Model NewModel)
        {
            try
            {
                JObject JSONResponse = PostJSONObject(URL, NewModel.ToJSON());

                return Model.CreateModel(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the model: " + e.Message, e);
            }
        }

        /// <summary>
        /// Calls the API to create a new Model
        /// </summary>
        /// <param name="NewModel"></param>
        /// <returns></returns>
        public Model CreateModel(Model NewModel)
        {
            return CreateModel(DefaultUser.modelsUrl, NewModel);
        }

        /// <summary>
        /// Calls the API to retrieve a list of Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Model> RetrieveModels(string URL)
        {
            try
            {
                JObject JSONResponse = GetJSONObject(URL);

                //deserialize into a list
                List<Model> ModelList = JsonConvert.DeserializeObject<List<Model>>(JSONResponse["models"].ToString());

                //since we're just deserializing directly into model objects, we need to ensure that the API Client
                //is set on each model
                foreach (Model CurrentModel in ModelList)
                    CurrentModel.ModelAPIClient = this;

                return ModelList;

            }
            catch (Exception ex)
            {
                throw new APIException("Failed to extract the list of models", ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a specific Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Model RetrieveModel(string URL)
        {
            try
            {
                return Model.CreateModel(this, GetJSONObject(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve the project " + URL, ex);
            }
        }

        /// <summary>
        /// Calls the API to retrieve a specific Model
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Model RetrieveModelById(string ID)
        {
            String URL = "/" + _Version + "/models/" + ID;
            return RetrieveModel(URL);
        }

        /// <summary>
        /// Deletes the Model at the specified URL
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public bool DeleteModel(string URL)
        {
            JObject response = DeleteJSONObject(URL);
            if (response["errors"] != null)
            {
                throw new APIException("Error deleting Model");
            }

            return true;
        }

        /// <summary>
        /// Deletes the Model 
        /// </summary>
        /// <param name="ModelToDelete"></param>
        /// <returns></returns>
        public bool DeleteModel(Model ModelToDelete)
        {
            return DeleteModel(ModelToDelete.url);
        }

        /// <summary>
        /// Clone model based on the last checkpoint
        /// </summary>
        /// <param name="ModelToClone"></param>
        /// <returns></returns>
        public Model CloneModel(Model ModelToClone)
        {
            var json = PostJSONObject(ModelToClone.url+ "/clone", new JObject());
            Model clone = Model.CreateModel(this, json);
            return clone;
        }
        #endregion Model Methods
    }
}