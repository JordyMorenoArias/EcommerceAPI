using EcommerceAPI.Services.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;

namespace EcommerceAPI.Services
{
    /// <summary>
    /// Email service for account verification and password recovery.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly string _BaseURL;
        private readonly string _username;
        private readonly string _password;

        /// <summary>
        /// Initializes the email service with configuration values.
        /// </summary>
        /// <param name="configuration">Instance of <see cref="IConfiguration"/> to retrieve configuration values.</param>
        /// <exception cref="ArgumentNullException">Thrown if required configuration values are missing.</exception>
        public EmailService(IConfiguration configuration)
        {
            _BaseURL = configuration["AppSettings:BaseUrl"] ?? throw new ArgumentNullException("AppSettings:BaseUrl is required");
            _username = configuration["EmailSettings:Username"] ?? throw new ArgumentNullException("EmailSettings:Username is required");
            _password = configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("EmailSettings:Password is required");
        }

        /// <summary>
        /// Sends an account verification email to the specified user.
        /// </summary>
        /// <param name="email">Recipient's email address.</param>
        /// <param name="token">Verification token.</param>
        /// <returns>Returns <c>true</c> if the email was sent successfully; otherwise, <c>false</c>.</returns>
        public bool SendVerificationEmail(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EcommerceAPI", _username));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Email Confirmation";

            var verificationLink = $"{_BaseURL}/api/auth/confirm-email?token={token}&email={email}";

            message.Body = new TextPart("html")
            {
                Text = $"<h1>Email Confirmation</h1><p>Please confirm your email by clicking <a href='{verificationLink}'>here</a></p>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(_username, _password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }

        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <param name="email">Recipient's email address.</param>
        /// <returns>Returns <c>true</c> if the email was sent successfully; otherwise, <c>false</c>.</returns>
        public bool SendForgotPassword(string email, int code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("EcommerceAPI", _username));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Password Reset";

            message.Body = new TextPart("html")
            {
                Text = $"<h1>Password Reset</h1><p>Your password reset code is: {code}</p>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(_username, _password);
                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }
    }
}
