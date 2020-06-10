using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class RegisterCheckinRequest
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string IdRoute { get; set; }
        [DataMember]
        public string IdPromo { get; set; }
        
        [DataMember]
        public string IdStore { get; set; }
        [DataMember]
        public string IdDay { get; set; }
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public string IdMotive { get; set; }
        [DataMember]
        public bool ValidCheckin { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public DateTime DateCheckin { get; set; }
        [DataMember]
        public DateTime TimeCheckin { get; set; }
        
    }
}