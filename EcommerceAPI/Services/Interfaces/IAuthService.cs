using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.Security.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ChangePassword(int id, UserChangePasswordDto changePasswordDto);
        Task<bool> ConfirmEmail(string email, string token);
        Task<AuthResponseDto> Login(UserLoginDto loginDto);
        Task<bool> Register(UserRegisterDto userRegister);
        Task<bool> ResetPassword(UserResetPasswordDto userResetPassword);
        Task<bool> SendForgotPasswordEmail(string email);
        Task<bool> VerifyResetCode(UserVerifyResetCodeDto verifyResetCodeDto);
    }
}