using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherSearchApp_Service.Interfaces;
using WeatherSearchApp_Service.Models;

namespace WeatherSearchApp_Service.Services
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

        public bool GenerateJwtToken(LoginModel model, int id)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(ClaimTypes.Upn, model.Email.ToLower()),
                new Claim(JwtRegisteredClaimNames.UniqueName, model.Email.ToLower()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var tokenAndCookieExpiry = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationTime"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = tokenAndCookieExpiry,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var writedToken = new JwtSecurityTokenHandler().WriteToken(token);

            _contextAccessor.HttpContext.Response.Cookies.Append(_configuration["Jwt:Name"], writedToken,
                                                                new CookieOptions
                                                                {
                                                                    Expires = tokenAndCookieExpiry,
                                                                    HttpOnly = true,
                                                                    Secure = true,
                                                                    IsEssential = true,
                                                                    SameSite = SameSiteMode.None,
                                                                });

            return true;
        }
    }
}
