
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using product_manager_webapi.Data.Entities;
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

        /// <summary>
        /// Login.
        /// </summary>
        /// <remarks>
        /// This endpoints allows a user to authenticate.
        /// </remarks>
        /// <returns>
        /// If the authentication is successful, returns a JWT token that can be used as bearer for authorizing.
        /// If the authentication fails, returns a Unauthorized 401 response.
        /// </returns>
        /// <response code="200">Returns a JWT.</response>
        /// <response code="401">If authentication fails.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<TokenDto> Authenticate(AuthenticateRequestDto authenticateRequest)
        {
            var user = context
            .Users.FirstOrDefault(x => x.UserName == authenticateRequest.UserName
                && x.Password == authenticateRequest.Password);

            if (user is null)
            {
                return Unauthorized();
            }

            var tokenDto = GenerateToken(user);

            return tokenDto;
        }

        private TokenDto GenerateToken(User user)
        {
            var signingKey = Convert.FromBase64String("tKE+pMd2rQAHBbOjXWTZqacLJRLqlrnTzZdmKRJEXLjtiGOnFY3w+vuUxPSgLdMFbbVXxPrFWNUd/yQyG5PsEg==");
            // var signingKey = Convert.FromBase64String(config["JWT:SigningSecret"]);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName)
            };

            // Detta gör så att EF Core laddar alla associerade roller för användnaren

            context.Entry(user).Collection(b => b.Roles).Load();

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKey),
                SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims)
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