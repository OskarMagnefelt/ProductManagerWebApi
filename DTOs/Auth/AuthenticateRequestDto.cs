using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace product_manager_webapi.DTOs.Auth
{
    public class AuthenticateRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}