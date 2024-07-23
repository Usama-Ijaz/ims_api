using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IMS.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _config;
        public LoginService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<string> GenerateJwtToken(IMS.Models.User.User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims: new List<Claim>() { new Claim("UserId", Convert.ToString(user.UserId)), 
                  new Claim("UserName", Convert.ToString(user.UserName)) },
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(Sectoken);
        }
    }
}
