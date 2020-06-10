using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WS_PosData_PMKT.Models.Object;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class RegisterAuditRequest
    {
        
            [DataMember]
            public string IdSync { get; set; }
            [DataMember]
            public string IdStore { get; set; }
            [DataMember]
            public List<DataProductAudit> ListProductsAudit { get; set; }
            [DataMember]
            public DateTime StartEventDate { get; set; }
            [DataMember]
            public DateTime EndEventDate { get; set; }
       
    }
}