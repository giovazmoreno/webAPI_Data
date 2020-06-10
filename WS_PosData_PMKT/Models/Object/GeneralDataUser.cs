using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

 namespace WS_PosData_PMKT.Models.Object
{
    [DataContract]
    public class GeneralDataUser
    {
        [DataMember]
        public int IdUser { get; set; }

        [DataMember]
        public string NameEmployee { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string MothersLastName { get; set; }

        [DataMember]
        public string IdStaffType { get; set; }

        [DataMember]
        public string NameStaffType { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}