using EcommerceAPI.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        public JwtService(IConfiguration configuration)
        {
            _jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()!;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
{
                new Claim(JwtRegisteredClaimNames.Sub, _jwtOptions!.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("Id", user.Id.ToString()),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
