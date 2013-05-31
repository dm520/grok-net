// ----------------------------------------------------------------------
//  Copyright (C) 2006-2012 Numenta Inc. All rights reserved.
//
//  The information and source code contained herein is the
//  exclusive property of Numenta Inc. No part of this software
//  may be used, reproduced, stored or distributed in any form,
//  without explicit written authorization from Numenta Inc.
// ----------------------------------------------------------------------
using System;

using System.Runtime.Serialization.Json;

namespace Grok.Numenta
{
    /// <summary>
    /// The APIException class represents an error encountered when trying to execute an API command. 
    /// In the common case, these errors will only occur as a result of bad requests, 
    /// such as malformed inputs or referencing non-existent entities.
    /// </summary>
    public class APIException : Exception
    {
        public APIException() : base() { }

        public APIException(string Message) : base(Message) { }

        public APIException(string Message, Exception InnerException) : base(Message, InnerException) { }
    }
}
