using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.Auth.Interfaces
{
    /// <summary>
    /// Interface for authentication service that handles user authentication operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="changePasswordDto">The data transfer object containing the new password information.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the password was successfully changed.</returns>
        Task<bool> ChangePassword(int id, UserChangePasswordDto changePasswordDto);

        /// <summary>
        /// Confirms the email address of the user.
        /// </summary>
        /// <param name="email">The email address to be confirmed.</param>
        /// <param name="token">The token used for email confirmation.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the email was successfully confirmed.</returns>
        Task<bool> ConfirmEmail(string email, string token);

        /// <summary>
        /// Logs the user in using the provided credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing the user's credentials.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AuthResponseDto"/> containing the authentication details.</returns>
        Task<AuthResponseDto> Login(UserLoginDto loginDto);

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The registration data transfer object containing the user's registration information.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the registration was successful.</returns>
        Task<bool> Register(UserRegisterDto userRegister);

        /// <summary>
        /// Resets the password for the user.
        /// </summary>
        /// <param name="userResetPassword">The data transfer object containing the reset password information.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the password was successfully reset.</returns>
        Task<bool> ResetPassword(UserResetPasswordDto userResetPassword);

        /// <summary>
        /// Sends a forgot password email to the user.
        /// </summary>
        /// <param name="email">The email address to send the forgot password email to.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the email was successfully sent.</returns>
        Task<bool> SendForgotPasswordEmail(string email);

        /// <summary>
        /// Verifies the reset code for the user.
        /// </summary>
        /// <param name="verifyResetCodeDto">The data transfer object containing the reset code to be verified.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the reset code was successfully verified.</returns>
        Task<bool> VerifyResetCode(UserVerifyResetCodeDto verifyResetCodeDto);
    }
}