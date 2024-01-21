using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskTracker.DTO;

namespace TaskTracker.Services
{
    public class AuthenticateService:IAuthenticateService
    {
        private readonly IConfiguration _configuration;

        public AuthenticateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(LoginDto loginDto)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}



