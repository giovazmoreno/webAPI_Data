using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WS_PosData_PMKT.Helpers;

namespace WS_PosData_PMKT.Models.Request
{
    public class LoginUserRequest
    {
        [Insurable]
        public string Email { get; set; }
        [Insurable]
        public string Password { get; set; }
    }
}