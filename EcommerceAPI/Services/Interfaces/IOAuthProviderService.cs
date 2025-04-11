using EcommerceAPI.Models.DTOs.Auth;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IOAuthProviderService
    {
        Task<AuthResponseDto> GetUserInfoAsync(string code);
    }
}