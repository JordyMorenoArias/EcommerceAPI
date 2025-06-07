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
        /// Changes a user's password after validating the old password.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="oldPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating whether the password change was successful,
        /// along with an optional message describing the result.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the old password is incorrect.</exception>
        /// <exception cref="InvalidOperationException">Thrown if password update fails.</exception>
        Task<OperationResult> ChangePassword(int id, UserChangePasswordDto changePassword);

        /// <summary>
        /// Confirms the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="token">The token.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the email confirmation.</returns>
        Task<OperationResult> ConfirmEmail(string email, string token);

        /// <summary>
        /// Logs the user in using the provided credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing the user's credentials.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AuthResponseDto"/> containing the authentication details.</returns>
        Task<AuthResponseDto> Login(UserLoginDto loginDto);

        /// <summary>
        /// Registers the specified user register.
        /// </summary>
        /// <param name="userRegister">The user register.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the registration process.</returns>
        Task<OperationResult> Register(UserRegisterDto userRegister);

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="userResetPassword">The user reset password.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the password reset operation.</returns>
        Task<OperationResult> ResetPassword(UserResetPasswordDto userResetPassword);

        /// <summary>
        /// Sends a forgot password email to the user.
        /// </summary>
        /// <param name="email">The email address to send the forgot password email to.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating if the email was successfully sent.</returns>
        Task<OperationResult> SendForgotPasswordEmail(string email);

        /// <summary>
        /// Verifies the reset code.
        /// </summary>
        /// <param name="verifyResetCodeDto">The verify reset code dto.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating whether the reset code is valid.</returns>
        Task<OperationResult> VerifyResetCode(UserVerifyResetCodeDto verifyResetCodeDto);
    }
}