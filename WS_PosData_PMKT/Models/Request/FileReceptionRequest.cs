using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WS_PosData_PMKT.Models.Request
{
    [DataContract]
    public class FileReceptionRequest
    {
        [DataMember]
        public string IdSync { get; set; }
        [DataMember]
        public string CodeTypeFile { get; set; }

        [DataMember]
        public Byte[] FileToSave { get; set; }
    }
}