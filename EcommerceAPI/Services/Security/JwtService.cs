using EcommerceAPI.Configurations;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Services.Security
{
    /// <summary>
    /// Service for generating JWT tokens for user authentication and authorization.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Security.Interfaces.IJwtService" />
    public class JwtService : IJwtService
    {
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtService"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration from which JWT options are loaded.</param>
        public JwtService(IConfiguration configuration)
        {
            _jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>()!;
        }

        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user.
        /// </summary>
        /// <param name="user">The user information used to generate claims in the token.</param>
        /// <returns>
        /// A signed JWT token string containing user claims and expiration information.
        /// </returns>
        public string GenerateJwtToken(UserGenerateTokenDto user)
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