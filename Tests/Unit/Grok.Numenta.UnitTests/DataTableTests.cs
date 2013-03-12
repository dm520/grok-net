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
using NUnit.Framework;

using Newtonsoft.Json.Linq;

using Grok.Numenta;

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class DataTableTests
    {
        private void ValidateDataTableFromJSON(DataTable DTUnderTest)
        {
            Assert.AreEqual(DTUnderTest.Names.Count, 3);
            Assert.AreEqual(DTUnderTest.Names[0], "ROWID");
            Assert.AreEqual(DTUnderTest.Data.Count, 6);
            Assert.AreEqual(DTUnderTest.Data[0].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[1].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[2].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[3].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[4].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[5].Length, 3);
            Assert.AreEqual(DTUnderTest.Data[0][0], "0");
            Assert.AreEqual(DTUnderTest.Data[1][2], "6.1");
            Assert.AreEqual(DTUnderTest.Data[2][1], "2010-10-01 00:30:00.000000");
        }

        [Test]
        public void TestConstructor_JSONInput_ModelData()
        {
            JObject JSONObject = JObject.Parse(_OutputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            ValidateDataTableFromJSON(DTUnderTest);
        }

        [Test]
        public void TestConstructor_EmptyJSONInput_ModelData()
        {
            JObject JSONObject = JObject.Parse(_EmptyDataJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.IsEmpty(DTUnderTest.Names);
            Assert.IsEmpty(DTUnderTest.Data);
        }

        [Test]
        public void TestConstructor_JSONInput_StreamData()
        {
            JObject JSONObject = JObject.Parse(_InputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            ValidateDataTableFromJSON(DTUnderTest);
        }

        [Test]
        public void TestConstructor_2DStringArray()
        {
            string[][] recordsToStream = {
					    new string [] { "2011-01-01 00:00:00.0", "5.3" },
					    new string [] { "2011-01-01 00:15:00.0", "5.5" },
					    new string [] { "2011-01-01 00:30:00.0", "5.1" },
					    new string [] { "2011-01-01 00:45:00.0", "5.3" },
					    new string [] { "2011-01-01 01:00:00.0", "5.2" },
					    new string [] { "2011-01-01 01:15:00.0", "5.5" } };

            DataTable DTUnderTest = new DataTable(recordsToStream);

            Assert.IsEmpty(DTUnderTest.Names);
            Assert.AreEqual(DTUnderTest.Data.Count, 6);
            Assert.AreEqual(DTUnderTest.Data[0].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[1].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[2].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[3].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[4].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[5].Length, 2);
            Assert.AreEqual(DTUnderTest.Data[0][0], "2011-01-01 00:00:00.0");
            Assert.AreEqual(DTUnderTest.Data[2][1], "5.1");
        }

        [Test]
        public void TestConstructor_Default()
        {
            DataTable DTUnderTest = new DataTable();

            Assert.IsEmpty(DTUnderTest.Names);
            Assert.IsEmpty(DTUnderTest.Data);
        }

        [Test]
        public void TestGetLastRowID_EmptyDataTable()
        {
            DataTable DTUnderTest = new DataTable();

            Assert.AreEqual(DTUnderTest.GetLastRowId(), 0);
        }

        [Test]
        public void TestGetLastRowID_PopulatedDataTable()
        {
            JObject JSONObject = JObject.Parse(_OutputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.AreEqual(DTUnderTest.GetLastRowId(), 5);
        }

        [Test]
        public void TestGetLastRowID_InvalidDataTable()
        {
            string[][] recordsToStream = {
					    new string [] { "2011-01-01 00:00:00.0", "5.3" },
					    new string [] { "2011-01-01 00:15:00.0", "5.5" },
					    new string [] { "2011-01-01 00:30:00.0", "5.1" },
					    new string [] { "2011-01-01 00:45:00.0", "5.3" },
					    new string [] { "2011-01-01 01:00:00.0", "5.2" },
					    new string [] { "2011-01-01 01:15:00.0", "5.5" } };

            DataTable DTUnderTest = new DataTable(recordsToStream);

            Assert.AreEqual(DTUnderTest.GetLastRowId(), 0);
        }

        [Test]
        public void TestCustomAccessor_KeyValue_Exists()
        {
            JObject JSONObject = JObject.Parse(_InputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.AreEqual(DTUnderTest["ROWID"][0], "0");
            Assert.AreEqual(DTUnderTest["timestamp"][1], "2010-10-01 00:15:00.000000");
            Assert.AreEqual(DTUnderTest["consumption"][2], "5.9");
        }

        [Test]
        public void TestCustomAccessor_IndexValue_Exists()
        {
            JObject JSONObject = JObject.Parse(_InputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.AreEqual(DTUnderTest[0][0], "0");
            Assert.AreEqual(DTUnderTest[1][1], "2010-10-01 00:15:00.000000");
            Assert.AreEqual(DTUnderTest[2][2], "5.9");
        }

        [Test]
        public void TestCustomAccessor_KeyValue_DoesntExist()
        {
            JObject JSONObject = JObject.Parse(_InputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.Throws<KeyNotFoundException>(delegate { List<string> Test = DTUnderTest["KeyNotFound"]; });
        }

        [Test]
        public void TestCustomAccessor_IndexValue_DoesntExist()
        {
            JObject JSONObject = JObject.Parse(_InputJSON);

            DataTable DTUnderTest = new DataTable(JSONObject);

            Assert.Throws<KeyNotFoundException>(delegate { List<string> Test = DTUnderTest[4]; });
        }
        [Test]
        public void TestMetaPropertiesAccess()
        {
            JObject JSONObject = JObject.Parse(_OutputJSON);
            DataTable DTUnderTest = new DataTable(JSONObject);
            Assert.AreEqual(1, DTUnderTest.TimestampIndex);
            Assert.AreEqual(2, DTUnderTest.PredictedFieldIndex);
            Assert.AreEqual(7, DTUnderTest.PredictionFieldIndex);
        }
        private static string _TestData = @"""data"": [
                                              [
                                                0, 
                                                ""2010-10-01 00:00:00.000000"", 
                                                ""2.2""
                                              ], 
                                              [
                                                1, 
                                                ""2010-10-01 00:15:00.000000"", 
                                                ""6.1""
                                              ], 
                                              [
                                                2, 
                                                ""2010-10-01 00:30:00.000000"", 
                                                ""5.9""
                                              ], 
                                              [
                                                3, 
                                                ""2010-10-01 00:45:00.000000"", 
                                                ""6.3""
                                              ], 
                                              [
                                                4, 
                                                ""2010-10-01 01:00:00.000000"", 
                                                ""5.8""
                                              ], 
                                              [
                                                5, 
                                                ""2010-10-01 01:15:00.000000"", 
                                                ""1.7""
                                              ]
                                            ], 
                                            ""names"": [
                                              ""ROWID"", 
                                              ""timestamp"", 
                                              ""consumption""
                                            ]";

        private static string _InputJSON = @"{ ""input"": {" + _TestData + @"}, 
                                          ""ok"": true
                                        }";

        private static string _OutputJSON = @"{ ""output"": {
                                                ""meta"": {
                                                  ""engineModelId"": 35149, 
                                                  ""timestampIndex"": 1, 
                                                  ""engineJobId"": 3054, 
                                                  ""predictedFieldPredictionIndex"": 7, 
                                                  ""modelStatus"": ""running"", 
                                                  ""predictedFieldIndex"": 2
                                                }, " + _TestData + @" },
                                              ""ok"": true
                                            }";

        private static string _EmptyDataJSON = @"{""output"": {
                                                  ""meta"": {
                                                    ""engineModelId"": 53780, 
                                                    ""engineJobId"": 1408, 
                                                    ""predictedFieldPredictionIndex"": -1, 
                                                    ""modelStatus"": ""running"", 
                                                    ""predictedFieldIndex"": -1
                                                   }, ""data"": [], ""names"": []}, 
                                                 ""ok"": true
                                                }";
    }
}
