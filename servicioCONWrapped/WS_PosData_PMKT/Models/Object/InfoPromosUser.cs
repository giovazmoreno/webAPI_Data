using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class InfoPromosUser
    {
        [DataMember]
        public string IdClient { get; set; }

        [DataMember]
        public string NameClient { get; set; }

        [DataMember]
        public string IdPromo { get; set; }

        [DataMember]
        public string NamePromo { get; set; }

        [DataMember]
        public List<Events> AvailableEvents { get; set; }

        [DataMember]
        public bool ValidateCheckIn { get; set; }
        
    }

    public class Events
    {
        public string IdTypeEvent { get; set; }
        public string NameTypeEvent { get; set; }
    }
    
}