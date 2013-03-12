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
    public class DataSourceFieldTests
    {
        [Test]
        public void TestConstructor_SingleStringInput()
        {
            DataSourceField DSFUnderTest = new DataSourceField("Test Name");

            Assert.AreEqual(DSFUnderTest.name, "Test Name");
            Assert.AreEqual(DSFUnderTest.dataType, DataSourceField.TYPE_SCALAR);
        }

        [Test]
        public void TestConstructor_StringStringInput()
        {
            DataSourceField DSFUnderTest = new DataSourceField("Test Name", DataSourceField.TYPE_SCALAR);

            Assert.AreEqual(DSFUnderTest.name, "Test Name");
            Assert.AreEqual(DSFUnderTest.dataType, DataSourceField.TYPE_SCALAR);
            Assert.AreEqual(DSFUnderTest.flag, DataSourceField.FLAG_NONE);
        }

        [Test]
        public void TestConstructor_StringStringStringInput()
        {
            DataSourceField DSFUnderTest = new DataSourceField("Test Name", DataSourceField.TYPE_SCALAR, DataSourceField.FLAG_NONE);

            Assert.AreEqual(DSFUnderTest.name, "Test Name");
            Assert.AreEqual(DSFUnderTest.dataType, DataSourceField.TYPE_SCALAR);
            Assert.AreEqual(DSFUnderTest.flag, DataSourceField.FLAG_NONE);
        }

        [Test]
        public void TestConstructor_StringDoubleDoubleInput()
        {
            DataSourceField DSFUnderTest = new DataSourceField("Test Name", 0, 10);

            Assert.AreEqual(DSFUnderTest.name, "Test Name");
            Assert.AreEqual(DSFUnderTest.dataType, DataSourceField.TYPE_SCALAR);
            Assert.AreEqual(DSFUnderTest.flag, DataSourceField.FLAG_NONE);
            Assert.AreEqual(DSFUnderTest.min, 0);
            Assert.AreEqual(DSFUnderTest.max, 10);
        }

        [Test]
        public void TestConstructor_JSONInput()
        {
            string JSON = @"{
                                flag: ""TIMESTAMP"",
                                dataFormat: {
                                          ""dataType"": ""DATETIME"", 
                                          ""formatString"": ""sdf/yyyy-MM-dd H:m:s.S""
                                },
                                name: ""timestamp""
                                }";
            JObject JSONObject = JObject.Parse(JSON);

            DataSourceField DSFUnderTest = new DataSourceField(JSONObject);

            Assert.AreEqual(DSFUnderTest.name, "timestamp");
            Assert.AreEqual(DSFUnderTest.dataType, DataSourceField.TYPE_DATETIME);
            Assert.AreEqual(DSFUnderTest.formatString, "sdf/yyyy-MM-dd H:m:s.S");
            Assert.AreEqual(DSFUnderTest.flag, DataSourceField.FLAG_TIMESTAMP);
        }

        [Test]
        public void TestToJSON()
        {
            DataSourceField DSFUnderTest = new DataSourceField("Test Name", 0, 10);
            JObject JSONObject = DSFUnderTest.ToJSON();

            Assert.AreEqual((string)JSONObject["name"], "Test Name");
            Assert.AreEqual((string)JSONObject["dataFormat"]["dataType"], DataSourceField.TYPE_SCALAR);
            Assert.AreEqual((string)JSONObject["flag"], DataSourceField.FLAG_NONE);
            Assert.AreEqual((double)JSONObject["min"], 0);
            Assert.AreEqual((double)JSONObject["max"], 10);
            Assert.IsNull(JSONObject["dataFormat"]["formatString"]);
        }
    }
}
