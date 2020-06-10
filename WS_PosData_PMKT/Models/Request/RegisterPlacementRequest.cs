using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WS_PosData_PMKT.Models.Object;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class RegisterPlacementRequest
    {
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public string IdStore { get; set; }
        [DataMember]
        public List<DataMaterialPlacement> ListMaterialPlacement { get; set; }
        [DataMember]
        public DateTime StartEventDate { get; set; }
        [DataMember]
        public DateTime EndEventDate { get; set; }
    }
}