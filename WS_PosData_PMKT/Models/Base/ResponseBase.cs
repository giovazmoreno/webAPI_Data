﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Base
{
    [DataContract]
    public class ResponseBase
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

}