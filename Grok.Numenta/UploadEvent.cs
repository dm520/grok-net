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
using System.Text;
using System.Threading.Tasks;

namespace Grok.Numenta
{
    /// <summary>
    /// Author: Jared Casner
    /// Last Updated: 19 November, 2012
    /// Class: UploadEvent
    /// Description: Event to call during Upload to track the progress of the upload
    /// </summary>
    public class UploadEvent
    {
        private double _PercentComplete;
	
	    public UploadEvent(double PercentComplete) 
        {
            this._PercentComplete = PercentComplete;
	    }

        public double PercentComplete
        {
            get { return _PercentComplete; }
            set { _PercentComplete = value; }
        }

        /// <summary>
        /// Author: Jared Casner
        /// Last Updated: 19 November, 2012
        /// Method: ToString
        /// Description: Returns a string representation of the upload percentage
        /// </summary>
        /// <returns></returns>
	    public override string ToString() 
        {
            return Math.Round(PercentComplete * 100, 2).ToString() + "%";
	    }
    }
}
