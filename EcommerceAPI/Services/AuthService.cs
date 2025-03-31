using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;
using EcommerceAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.Security;

namespace EcommerceAPI.Services
{
    /// <summary>
    /// Provides authentication services, including user login, registration, password management,
    /// and email verification.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, ITokenGenerator tokenGenerator, IEmailService emailService, PasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Logs in a user with the provided email and password.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>An authentication response containing a JWT token and user details.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the email or password is incorrect.</exception>
        public async Task<AuthResponseDto> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = _jwtService.GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(3),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName!,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role
                }
            };
        }

        /// <summary>
        /// Registers a new user and sends an email verification token.
        /// </summary>
        /// <param name="userRegister">User registration details.</param>
        /// <returns>True if registration is successful, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the email is already registered or registration fails.</exception>
        public async Task<bool> Register(UserRegisterDto userRegister)
        {
            var user = await _userRepository.GetUserByEmail(userRegister.Email);

            if (user != null)
                throw new InvalidOperationException("User with this email already exists");

            userRegister.Password = _passwordHasher.HashPassword(new User(), userRegister.Password);

            string token = _tokenGenerator.GenerateToken();

            User newUser = new User
            {
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                PhoneNumber = userRegister.PhoneNumber,
                Email = userRegister.Email,
                PasswordHash = userRegister.Password,
                EmailConfirmedToken = token
            };

            var result = await _userRepository.AddUser(newUser);

            if (!result)
                throw new InvalidOperationException("Failed to register user");


            var emailResult = _emailService.SendVerificationEmail(userRegister.Email, token);
            return emailResult;
        }

        /// <summary>
        /// Confirms a user's email using a verification token.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="token">Verification token.</param>
        /// <returns>True if email confirmation is successful, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the token is invalid or confirmation fails.</exception>
        public async Task<bool> ConfirmEmail(string email, string token)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null || user.EmailConfirmedToken != token)
                throw new InvalidOperationException("Invalid token");

            var result = await _userRepository.ConfirmUser(token);

            if (!result)
                throw new InvalidOperationException("Failed to confirm email");

            return result;
        }

        /// <summary>
        /// Changes a user's password after validating the old password.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="oldPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>True if the password is changed successfully, otherwise false.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the old password is incorrect.</exception>
        /// <exception cref="InvalidOperationException">Thrown if password update fails.</exception>
        public async Task<bool> ChangePassword(int id, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword) == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

            var result = await _userRepository.UpdateUser(user);

            if (!result)
                throw new InvalidOperationException("Failed to change password");

            return result;
        }

        /// <summary>
        /// Sends an email to reset the user's password.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>True if the email is sent successfully, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the email does not exist.</exception>
        public async Task<bool> SendForgotPasswordEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if(user is null)
                throw new InvalidOperationException("User with this email does not exist");

            var emailResult = _emailService.SendForgotPassword(email);
            return emailResult;
        }

        /// <summary>
        /// Generates a reset password token for the user.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>A password reset token.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the user does not exist or token generation fails.</exception>
        public async Task<string> GenerateResetToken(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user is null)
                throw new InvalidOperationException("User with this email does not exist");

            string token = _tokenGenerator.GenerateToken();

            user.ResetPasswordToken = token;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            var result = await _userRepository.UpdateUser(user);

            if (!result)
                throw new InvalidOperationException("Failed to generate reset token");

            return token;
        }

        ///    <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="token">Reset password token.</param>
        /// <param name="password">New password.</param>
        /// <returns>True if the password is reset successfully, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the token is invalid, expired, or reset fails.</exception>
        public async Task<bool> ResetPassword(string email, string token, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user is null || user.ResetPasswordToken != token || user.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invalid or expired token");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            user.ResetPasswordToken = null;
            user.ResetTokenExpiresAt = null;

            var result = await _userRepository.UpdateUser(user);

            if (!result)
                throw new InvalidOperationException("Failed to reset password");

            return result;
        }
    }
}
