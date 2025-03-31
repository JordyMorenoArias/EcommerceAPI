using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ChangePassword(int id, string oldPassword, string newPassword);
        Task<bool> ConfirmEmail(string email, string token);
        Task<string> GenerateResetToken(string email);
        Task<AuthResponseDto> Login(string email, string password);
        Task<bool> Register(UserRegisterDto userRegister);
        Task<bool> ResetPassword(string email, string token, string password);
        Task<bool> SendForgotPasswordEmail(string email);
    }
}