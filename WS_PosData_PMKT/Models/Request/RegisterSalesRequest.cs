using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WS_PosData_PMKT.Models.Object;
using System.Web;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class RegisterSalesRequest
    {
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public string IdStore { get; set; }
        [DataMember]
        public List<DataProductKPI> ListProductsKPI { get; set; }
        [DataMember]
        public DateTime StartEventDate { get; set; }
        [DataMember]
        public DateTime EndEventDate { get; set; }
    }
}