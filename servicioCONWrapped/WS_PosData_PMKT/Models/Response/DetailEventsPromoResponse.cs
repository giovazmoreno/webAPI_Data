using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WS_PosData_PMKT.Models.Base;
using WS_PosData_PMKT.Models.Object;

namespace WS_PosData_PMKT.Models.Response
{
    public class DetailEventsPromoResponse : ResponseBase
    {
        public DetailEventsPromo DetailEvents { get; set; }
    }
}