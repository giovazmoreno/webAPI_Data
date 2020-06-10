using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class RegisterSurveyRequest
    {
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public string IdStore { get; set; }
        [DataMember]
        public string IdSurvey { get; set; }
        [DataMember]
        public string CommentsCheckout { get; set; }
        [DataMember]
        public DateTime DateTimeStartEvent { get; set; }
        [DataMember]
        public DateTime DateTimeEndEvent { get; set; }
        [DataMember]
        public bool Answer1 { get; set; }
        [DataMember]
        public bool Answer2 { get; set; }
        [DataMember]
        public bool Answer3 { get; set; }
        [DataMember]
        public bool Answer4 { get; set; }
        [DataMember]
        public bool Answer5 { get; set; }
        [DataMember]
        public bool Answer6 { get; set; }
        [DataMember]
        public bool Answer7 { get; set; }
        [DataMember]
        public bool Answer8 { get; set; }
        [DataMember]
        public bool Answer9 { get; set; }
        [DataMember]
        public bool Answer10 { get; set; }
        [DataMember]
        public bool Answer11 { get; set; }
        [DataMember]
        public bool Answer12 { get; set; }
        [DataMember]
        public bool Answer13 { get; set; }
        [DataMember]
        public bool Answer14 { get; set; }
        [DataMember]
        public bool Answer15 { get; set; }
    }
}