using EcommerceAPI.Services.Security.Interfaces;
using System.Security.Cryptography;

namespace EcommerceAPI.Services.Security
{
    public class TokenGenerator : ITokenGenerator
    {
        public string GenerateToken()
        {
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
            string token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-") 
                .Replace("/", "_")
                .TrimEnd('=');

            return token;
        }

        public int Generate6DigitToken()
        {
            int code = RandomNumberGenerator.GetInt32(100000, 1000000);
            return code;
        }
    }
}
