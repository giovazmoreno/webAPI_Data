using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Request
{
    public class RegisterCheckoutRequest
    {
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public bool ValidCheckout { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public DateTime DateCheckout { get; set; }
        [DataMember]
        public DateTime TimeCheckout { get; set; }
        [DataMember]
        public string CommentsCheckout { get; set; }
    }
}