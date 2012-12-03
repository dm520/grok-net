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

namespace Grok.Numenta.UnitTests
{
    [TestFixture]
    public class UploadEventTests
    {
        [Test]
        public void TestToString()
        {
            UploadEvent Event = new UploadEvent(.4334);
            Assert.AreEqual(Event.ToString(), "43.34%");

            Event = new UploadEvent(.43);
            Assert.AreEqual(Event.ToString(), "43%");

            Event = new UploadEvent(.561234);
            Assert.AreEqual(Event.ToString(), "56.12%");

            Event = new UploadEvent(0.0);
            Assert.AreEqual(Event.ToString(), "0%");

            Event = new UploadEvent(1);
            Assert.AreEqual(Event.ToString(), "100%");
        }
    }
}
