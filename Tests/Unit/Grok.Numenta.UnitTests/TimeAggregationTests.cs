// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;
using NUnit.Framework;

using Newtonsoft.Json.Linq;

using Grok.Numenta;

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class TimeAggregationTests
    {
        [Test]
        public void TestJSONConstructor_NoFieldOverrides()
        {
            string JSON = @"{""aggregation"" : {
                                ""interval"": {
                                    ""hours"": 1
                                    }
                                }
                            }";
            JObject TAObject = JObject.Parse(JSON);

            TimeAggregation AggregationUnderTest = new TimeAggregation(TAObject);

            Assert.AreEqual(AggregationUnderTest.interval["hours"], 1);
            Assert.AreEqual(AggregationUnderTest.FieldOverrides.Count, 0);
            Assert.AreEqual(AggregationUnderTest.GetHours(), 1);
        }

        [Test]
        public void TestJSONConstructor_FieldOverrides()
        {
            string JSON = @"{""aggregation"" : {
                                ""fields"": [
                                        [
                                          ""consumption"",
                                          ""mean""
                                        ]
                                      ],
                                ""interval"": {
                                    ""hours"": 1
                                    }
                                }
                            }";
            JObject TAObject = JObject.Parse(JSON);

            TimeAggregation AggregationUnderTest = new TimeAggregation(TAObject);

            Assert.AreEqual(AggregationUnderTest.interval["hours"], 1);
            Assert.AreEqual(AggregationUnderTest.FieldOverrides[0], new string[] {"consumption", "mean"} );
            Assert.AreEqual(AggregationUnderTest.GetHours(), 1);
        }

        [Test]
        public void TestConstructor_NoFieldOverrides()
        {
            TimeAggregation AggregationUnderTest = new TimeAggregation(TimeAggregation.INTERVAL_HOURS);

            Assert.AreEqual(AggregationUnderTest.interval["hours"], 1);
            Assert.AreEqual(AggregationUnderTest.FieldOverrides.Count, 0);
            Assert.AreEqual(AggregationUnderTest.GetHours(), 1);
        }

        [Test]
        public void TestAddFieldOverride()
        {
            TimeAggregation AggregationUnderTest = new TimeAggregation(TimeAggregation.INTERVAL_HOURS);
            AggregationUnderTest.FieldOverrides.Add(new string[] { "consumption", TimeAggregation.FUNCTION_MEAN });

            Assert.AreEqual(AggregationUnderTest.interval["hours"], 1);
            Assert.AreEqual(AggregationUnderTest.FieldOverrides[0], new string[] { "consumption", "mean" });
            Assert.AreEqual(AggregationUnderTest.GetHours(), 1);
        }

        [Test]
        public void TestToJSON()
        {
            TimeAggregation AggregationUnderTest = new TimeAggregation(TimeAggregation.INTERVAL_HOURS);
            AggregationUnderTest.FieldOverrides.Add(new string[] { "consumption", TimeAggregation.FUNCTION_MEAN });

            JObject JSON = AggregationUnderTest.ToJSON();

            Assert.AreEqual((int)JSON["aggregation"]["interval"]["hours"], 1);
            Assert.AreEqual((string)JSON["aggregation"]["fields"][0][0], "consumption");
            Assert.AreEqual((string)JSON["aggregation"]["fields"][0][1], "mean");
        }
    }
}
