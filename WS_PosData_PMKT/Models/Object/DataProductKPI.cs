using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class DataProductKPI
    {
        [DataMember]
        public string IdProduct { get; set; }
        [DataMember]
        public string IdCategory { get; set; }
        [DataMember]
        public string IdKPI { get; set; }
        [DataMember]
        public string KPIValue { get; set; }
        [DataMember]
        public string ObservationsSale { get; set; }
    }
}