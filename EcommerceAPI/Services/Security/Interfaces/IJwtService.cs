using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Security.Interfaces
{
    /// <summary>
    /// Interface for JWT (JSON Web Token) generation service.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT token for a user.
        /// </summary>
        /// <param name="user">The user information used to generate the token.</param>
        /// <returns>A string representing the generated JWT token.</returns>
        string GenerateJwtToken(UserGenerateTokenDto user);
    }
}