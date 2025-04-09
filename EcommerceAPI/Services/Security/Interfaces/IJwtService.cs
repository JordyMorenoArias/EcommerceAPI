using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Security.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(UserGenerateTokenDto user);
    }
}