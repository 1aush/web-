using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetTask.Utilities
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public string CreateToken(Guid userId, string userName, string userRole, Guid userDepartment)
        {
            var claims = new[]
            {
                new Claim("UserID", userId.ToString()),
                new Claim("UserName", userName),
                new Claim("UserRole", userRole),
                new Claim("UserDepartment", userDepartment.ToString())
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signingCredentials = new SigningCredentials(secretKey, algorithm);
            var expires = Convert.ToDouble(_configuration["JWT:Expires"]);

            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(expires),
                signingCredentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        public Models.LoginUser ReadToken()
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            token = token.Substring(7);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
            JwtPayload payload = jwtToken.Payload;
            Guid userId = Guid.Empty;
            string userName = "";
            string userRole = "";
            Guid userDepartment = Guid.Empty;
            if (payload.ContainsKey("UserID"))
            {
                try
                {
                    userId = new Guid(payload["UserID"].ToString());
                }
                catch { }
            }
            if (payload.ContainsKey("UserName"))
            {
                userName = payload["UserName"].ToString();
            }
            if (payload.ContainsKey("UserRole"))
            {
                userRole = payload["UserRole"].ToString();
            }
            if (payload.ContainsKey("UserDepartment"))
            {
                try
                {
                    userDepartment = new Guid(payload["UserDepartment"].ToString());
                }
                catch { }
            }
            return new Models.LoginUser
            {
                LoginUser_Id = userId,
                LoginUser_Account = userName,
                LoginUser_Role = userRole,
                LoginUser_Department = userDepartment
            };
        }
    }
}
