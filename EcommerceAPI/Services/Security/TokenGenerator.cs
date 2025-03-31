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
    }
}
