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
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 20 November, 2012
    /// Interface: IAPIClient
    /// Description: Interface for the API Client which allows us to be able to mock the API Client for testing purposes
    /// </summary>
    public interface IAPIClient
    {
        List<User> RetrieveUsers();
        User UpdateUser(User SingleUser);

        Project CreateProject(string URL, Project NewProject);
        Project CreateProject(Project NewProject);
        Project CreateProject(string ProjectName);
        List<Project> RetrieveProjects();
        List<Project> RetrieveProjects(string URL);
        Project RetrieveProject(string URL);
        Project RetrieveProject(Project CurrentProject);

        Stream CreateStream(string URL, Stream NewStream);
        Stream CreateStream(Stream NewStream);
        List<Stream> RetrieveStreams(string URL);
        Stream RetrieveStream(string URL);
        Stream RetrieveStreamById(string StreamID);
        void AppendData(string URL, List<string[]> Data, UploadCallback Callback);
        void AppendData(string URL, List<string[]> Data);

        Swarm CreateSwarm(string URL);
        Swarm RetrieveSwarm(string URL);

        JObject SendModelCommand(string URL, string Command, JObject Parameters);
        JObject SendModelCommand(string URL, string Command, Dictionary<string, string> Parameters);
        JObject SendModelCommand(string URL, string Command);
        JObject SendModelCommand(Model CurrentModel, string Command, Dictionary<string, string> Parameters);
        JObject SendModelCommand(Model CurrentModel, string Command);
        DataTable RetrieveOutputData(string URL);
        Model CreateModel(string URL, Model NewModel);
        Model CreateModel(Model NewModel);
        List<Model> RetrieveModels(string URL);
        Model RetrieveModel(string URL);
        Model RetrieveModelById(string ID);
        Model CloneModel(Model ModelToClone);
    }
}
