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
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 20 November, 2012
    /// Class: APIClient
    /// Description: Default API Client so that we can connect to Grok
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
        public string APIKey
        {
            get { return _APIKey; }
            set { _APIKey = value; }
        }

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

        public string VersionNumber 
        {
            get 
            {
                _CurrentAssembly = this.GetType().Assembly.GetName();
                _AssemblyVersion = _CurrentAssembly.Version;
                return _AssemblyVersion.ToString(4);
            }
        }

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
        #endregion Accessors
        #endregion Members and Accessors

        #region Constructors
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: APIClient
        /// Description: Constructor taking the API Key of the User 
        /// </summary>
        /// <param name="UserAPIKey"></param>
        public APIClient(string UserAPIKey) : this(UserAPIKey, _NumentaURI) { }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: APIClient
        /// Description: Constructor taking the API Key of the User and a base URL (e.g.: https://api.numenta.com)
        /// </summary>
        /// <param name="UserAPIKey"></param>
        /// <param name="ConnectionURL"></param>
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
        }
        #endregion Constructors

        #region HTTP Calls
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: Get
        /// Description: Makes an HTTP GET Call to the specified URL
        /// </summary>
        /// <param name="URL">Either an absolute or a relative URL</param>
        /// <returns></returns>
        protected virtual HttpResponseMessage Get(string URL) 
        {
		    try 
            {
                if (!URL.ToUpper().StartsWith("HTTP"))
                    URL = NumentaURI + URL;
                HttpResponseMessage response = _HttpClient.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new IOException("Failed to connect to " + NumentaURI + " with error " + (int)response.StatusCode + response.ReasonPhrase);
                }
		    } 
            catch (IOException ex) 
            {
			    throw new APIException("Failed to retrieve data from " + URL, ex);
		    }
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: PostJSONObject
        /// Description: Makes an HTTP POST Call to the specified URL with an attached JSON object
        /// </summary>
        /// <param name="URL">Either an absolute or a relative URL</param>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected virtual HttpResponseMessage PostJSONObject(string URL, JObject JSONObject)
        {
            try
            {
                StringContent PutContent = new StringContent(JSONObject.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage response = _HttpClient.PostAsync(URL, PutContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    throw new IOException("Failed to connect to " + NumentaURI + " with error " + (int)response.StatusCode + response.ReasonPhrase);
                }
            }
            catch (IOException ex)
            {
                throw new APIException("Failed to POST data: " + JSONObject.ToString(), ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteJSONObject
        /// Description: Deletes a resource at the specified URL.
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        protected virtual JObject DeleteJSONObject(string URL)
        {
            try
            {
                return new JObject(Delete(URL));
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to delete the object", ex);
            }
        }
	
	    /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: Delete
        /// Description: Deletes the resource at the specified URL.
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
                        return response.Content.ReadAsStringAsync().Result;
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveUsers
        /// Description: retrieves a list of users
	    /// </summary>
	    /// <returns></returns>
        public List<User> RetrieveUsers()
        {
            try
            {
                HttpResponseMessage response = Get(_Version + "/users");

                //get the users
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

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
                throw new APIException("Failed to extract the list of users", ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: InitDefaultUser
        /// Description: Initializes the default user.
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: UpdateUser
        /// Description: The API expects a JSON Array, so we need to handle that appropriately before serializing
        /// </summary>
        /// <param name="SingleUser"></param>
        /// <returns></returns>
        public User UpdateUser(User SingleUser)
        {
            try
            {
                HttpResponseMessage response = PostJSONObject(SingleUser.url, SingleUser.ToJSON());

                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateProject
        /// Description: Calls the API to create a new Project 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewProject"></param>
        /// <returns></returns>
        public Project CreateProject(string URL, Project NewProject)
        {
            try
            {
                HttpResponseMessage response = PostJSONObject(URL, NewProject.ToJSON());

                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Project.CreateProject(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to create the project", ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateProject
        /// Description: Calls the API to create a new Project 
        /// </summary>
        /// <param name="NewProject"></param>
        /// <returns></returns>
        public Project CreateProject(Project NewProject)
        {
            return CreateProject(this.DefaultUser.projectsUrl, NewProject);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateProject
        /// Description: Calls the API to create a new Project 
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveProjects
        /// Description: Calls the API to retrieve a list of Projects 
        /// </summary>
        /// <returns></returns>
        public List<Project> RetrieveProjects()
        {
            return RetrieveProjects(this.DefaultUser.projectsUrl);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveProjects
        /// Description: Calls the API to retrieve a list of Projects 
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Project> RetrieveProjects(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveProject
        /// Description: Calls the API to retrieve a specific Project
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Project RetrieveProject(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                
                return Project.CreateProject(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve the project " + URL, ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveProject
        /// Description: Calls the API to retrieve a specific Project
        /// </summary>
        /// <param name="CurrentProject"></param>
        /// <returns></returns>
        public Project RetrieveProject(Project CurrentProject)
        {
            return RetrieveProject(CurrentProject.url);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteProject
        /// Description: Deletes the project at the specified URL
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
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteProject
        /// Description: Deletes the project 
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateStream
        /// Description: Calls the API to create a Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewStream"></param>
        /// <returns></returns>
        public Stream CreateStream(string URL, Stream NewStream)
        {
            try
            {
                HttpResponseMessage response = PostJSONObject(URL, NewStream.ToJSON());

                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Stream.CreateStream(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Stream", e);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateStream
        /// Description: Calls the API to create a Stream
        /// </summary>
        /// <param name="NewStream"></param>
        /// <returns></returns>
        public Stream CreateStream(Stream NewStream)
        {
            return CreateStream(DefaultUser.streamsUrl, NewStream);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveStreams
        /// Description: Calls the API to retrieve a list of Streams
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Stream> RetrieveStreams(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveStream
        /// Description: Calls the API to retrieve a specific Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Stream RetrieveStream(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Stream.CreateStream(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve stream " + URL, ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveStreamById
        /// Description: Calls the API to retrieve a list of Streams
        /// </summary>
        /// <param name="StreamID"></param>
        /// <returns></returns>
        public Stream RetrieveStreamById(string StreamID)
        {
            String URL = "/" + _Version + "/streams/" + StreamID;
            return RetrieveStream(URL);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: AppendData
        /// Description: Calls the API to append data to a Stream
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

                    HttpResponseMessage response = PostJSONObject(URL, request);

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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: AppendData
        /// Description: Calls the API to append data to a Stream
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Data"></param>
        public void AppendData(string URL, List<string[]> Data)
        {
            AppendData(URL, Data, null);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteStream
        /// Description: Deletes the Stream at the specified URL
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
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteStream
        /// Description: Deletes the Stream 
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveSwarm
        /// Description: Calls the API to retrieve a specific Swarm
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Swarm RetrieveSwarm(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Swarm.CreateSwarm(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve stream " + URL, ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateSwarm
        /// Description: Calls the API to create a Swarm
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Swarm CreateSwarm(string URL)
        {
            try
            {
                HttpResponseMessage response = PostJSONObject(URL, new JObject());

                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Swarm.CreateSwarm(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the Swarm", e);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteSwarm
        /// Description: Deletes the Swarm at the specified URL
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
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteSwarm
        /// Description: Deletes the Swarm 
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: SendModelCommand
        /// Description: Calls the API to send a command for a Model
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
			
			    HttpResponseMessage response = PostJSONObject(URL, request);
                
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);
			
			    return JSONResponse;
		    } 
            catch (Exception ex) 
            {
			    throw new APIException("Error while sending Model Command " + Command, ex);
		    }
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: SendModelCommand
        /// Description: Calls the API to send a command for a Model
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: SendModelCommand
        /// Description: Calls the API to send a command for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
	    public JObject SendModelCommand(string URL, string Command) 
        {
            return SendModelCommand(URL, Command, (JObject)null);
	    }
	
        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: SendModelCommand
        /// Description: Calls the API to send a command for a Model
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: SendModelCommand
        /// Description: Calls the API to send a command for a Model
        /// </summary>
        /// <param name="CurrentModel"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public JObject SendModelCommand(Model CurrentModel, string Command)
        {
		    return SendModelCommand(CurrentModel, Command, null);
	    }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveOutputData
        /// Description: Calls the API to retrieve output data for a Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public DataTable RetrieveOutputData(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);
                                
                return new DataTable(JObject.Parse(response.Content.ReadAsStringAsync().Result));
            }
            catch (Exception ex)
            {
                throw new APIException("Error retrieving Data Table", ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateModel
        /// Description: Calls the API to create a new Model
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="NewModel"></param>
        /// <returns></returns>
        public Model CreateModel(string URL, Model NewModel)
        {
            try
            {
                HttpResponseMessage response = PostJSONObject(URL, NewModel.ToJSON());

                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Model.CreateModel(this, JSONResponse);
            }
            catch (Exception e)
            {
                throw new APIException("Failed to create the model", e);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: CreateModel
        /// Description: Calls the API to create a new Model
        /// </summary>
        /// <param name="NewModel"></param>
        /// <returns></returns>
        public Model CreateModel(Model NewModel)
        {
            return CreateModel(DefaultUser.modelsUrl, NewModel);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveModels
        /// Description: Calls the API to retrieve a list of Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public List<Model> RetrieveModels(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                //deserialize into a list
                List<Model> ModelList = JsonConvert.DeserializeObject<List<Model>>(JSONResponse["models"].ToString());

                //since we're just deserializing directly into project objects, we need to ensure that the API Client
                //is set on each project
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
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveModel
        /// Description: Calls the API to retrieve a specific Model
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public Model RetrieveModel(string URL)
        {
            try
            {
                HttpResponseMessage response = Get(URL);

                //get the projects
                JObject JSONResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);

                return Model.CreateModel(this, JSONResponse);
            }
            catch (Exception ex)
            {
                throw new APIException("Failed to retrieve the project " + URL, ex);
            }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 20 November, 2012
        /// Method: RetrieveModelById
        /// Description: Calls the API to retrieve a specific Model
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Model RetrieveModelById(string ID)
        {
            String URL = "/" + _Version + "/models/" + ID;
            return RetrieveModel(URL);
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteModel
        /// Description: Deletes the Model at the specified URL
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
        /// Author: Jared Casner
        /// Last Updated: 26 November, 2012
        /// Method: DeleteModel
        /// Description: Deletes the Model 
        /// </summary>
        /// <param name="ModelToDelete"></param>
        /// <returns></returns>
        public bool DeleteModel(Model ModelToDelete)
        {
            return DeleteModel(ModelToDelete.url);
        }
        #endregion Model Methods
    }
}
