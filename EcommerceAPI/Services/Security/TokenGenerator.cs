using EcommerceAPI.Services.Security.Interfaces;
using System.Security.Cryptography;

namespace EcommerceAPI.Services.Security
{
    /// <summary>
    /// Generates secure tokens for authentication and verification purposes.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Security.Interfaces.ITokenGenerator" />
    public class TokenGenerator : ITokenGenerator
    {
        /// <summary>
        /// Generates a cryptographically secure random token string encoded in a URL-safe Base64 format.
        /// This can be used for purposes such as email verification, password reset links, or API keys.
        /// </summary>
        /// <returns>A secure, URL-safe Base64-encoded token string.</returns>
        public string GenerateToken()
        {
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
            string token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');

            return token;
        }

        /// <summary>
        /// Generates a cryptographically secure 6-digit numeric code.
        /// This is useful for two-factor authentication (2FA), account recovery, or other numeric verification purposes.
        /// </summary>
        /// <returns>A secure 6-digit integer between 100000 and 999999.</returns>
        public string Generate6DigitToken()
        {
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");
            return code;
        }
    }
}