using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using product_manager_webapi.DTOs.Auth;
using ProductManager.Data;

namespace product_manager_webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AuthController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]

        public ActionResult<TokenDto> Authenticate(AuthenticateRequestDto authenticateRequest)
        {
            var user = context
            .Users.FirstOrDefault(x => x.UserName == authenticateRequest.UserName
                && x.Password == authenticateRequest.Password);

            if (user is null)
            {
                return Unauthorized();
            }

            var tokenDto = GenerateToken();

            return tokenDto;
        }

        private TokenDto GenerateToken()
        {
            return new TokenDto { Token = "test" };
        }



    }
}