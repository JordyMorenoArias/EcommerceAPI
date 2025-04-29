namespace EcommerceAPI.Services.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for sending email-related services, such as forgot password and verification emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a forgot password email to the specified email address with a verification code.
        /// </summary>
        /// <param name="email">The email address to send the forgot password email to.</param>
        /// <param name="code">The verification code to be included in the email.</param>
        /// <returns>True if the email was sent successfully, otherwise false.</returns>
        bool SendForgotPassword(string email, int code);

        /// <summary>
        /// Sends a verification email to the specified email address with a token.
        /// </summary>
        /// <param name="email">The email address to send the verification email to.</param>
        /// <param name="token">The token to be included in the email for verification purposes.</param>
        /// <returns>True if the email was sent successfully, otherwise false.</returns>
        bool SendVerificationEmail(string email, string token);
    }
}