using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Security.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}