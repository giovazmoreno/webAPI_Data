using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class DataProductAudit
    {
        [DataMember]
        public string IdProduct { get; set; }
        [DataMember]
        public string IdCategory { get; set; }
        [DataMember]
        public int FinalInventory { get; set; }
        [DataMember]
        public decimal Price { get; set; }
        [DataMember]
        public int Position { get; set; }
        [DataMember]
        public int Front { get; set; }
        [DataMember]
        public string IdUbication { get; set; }
    }
}