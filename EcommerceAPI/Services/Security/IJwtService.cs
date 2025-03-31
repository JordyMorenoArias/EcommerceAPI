using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.Security
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}