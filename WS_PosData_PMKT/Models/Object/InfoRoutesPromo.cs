using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class InfoRoutesPromo
    {
        [DataMember]
        public string IdStore { get; set; }
        [DataMember]
        public string NameStore { get; set; }
        [DataMember]
        public string IdRegion { get; set; }
        [DataMember]
        public string NameRegion { get; set; }
        [DataMember]
        public string IdCity { get; set; }
        [DataMember]
        public string NameCity { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public string CheckInRange { get; set; }
    }

    [DataContract]
    public class GeneralDataRoute
    {
        [DataMember]
        public string IdRoute { get; set; }

        [DataMember]
        public string NameRoute { get; set; }
        [DataMember]
        public string IdRegion { get; set; }
        [DataMember]
        public string NameRegion { get; set; }
        [DataMember]
        public string IdCity { get; set; }
        [DataMember]
        public string NameCity { get; set; }
        [DataMember]
        public string NameSupervisor { get; set; }
        [DataMember]
        public List<InfoRoutesPromo> ListScheduledRoute { get; set; }
        [DataMember]
        public List<InfoRoutesPromo> ListScopePromoRoute { get; set; }
        [DataMember]
        public List<InfoRoutesPromo> ListVisitedStores { get; set; }
    }
}