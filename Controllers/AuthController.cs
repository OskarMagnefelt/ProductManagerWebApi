using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            var signingKey = Convert.FromBase64String("tKE+pMd2rQAHBbOjXWTZqacLJRLqlrnTzZdmKRJEXLjtiGOnFY3w+vuUxPSgLdMFbbVXxPrFWNUd/yQyG5PsEg==");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKey),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = jwtTokenHandler
              .CreateJwtSecurityToken(tokenDescriptor);

            var token = new TokenDto
            {
                // Generera token (t.ex. "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
                Token = jwtTokenHandler.WriteToken(jwtSecurityToken)
            };

            return token;
        }
    }
}