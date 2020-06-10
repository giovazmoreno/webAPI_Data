using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class DataMaterialPlacement
    {
        [DataMember]
        public string IdTypeMaterial { get; set; }
        [DataMember]
        public string New { get; set; }
        [DataMember]
        public string Amount { get; set; }
    }
}