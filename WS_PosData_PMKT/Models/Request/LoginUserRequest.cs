using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WS_PosData_PMKT.Helpers;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class LoginUserRequest
    {
        [DataMember]
        [Insurable]
        public string Email { get; set; }
        [DataMember]
        [Insurable]
        public string Password { get; set; }
        [DataMember]
        public DateTime getfechaactual { get; set; }
    }
}