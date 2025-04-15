using EcommerceAPI.Models.DTOs.Auth;

namespace EcommerceAPI.Services.Auth.Interfaces
{
    public interface IOAuthProviderService
    {
        Task<AuthResponseDto> GetUserInfoAsync(string code);
    }
}