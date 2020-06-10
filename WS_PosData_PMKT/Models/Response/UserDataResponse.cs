using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WS_PosData_PMKT.Models.Base;
using WS_PosData_PMKT.Models.Object;

namespace WS_PosData_PMKT.Models.Response
{
    
    public class UserDataResponse: ResponseBase
    { 
        
        public GeneralDataUser DataUser { get; set; }
    }
}