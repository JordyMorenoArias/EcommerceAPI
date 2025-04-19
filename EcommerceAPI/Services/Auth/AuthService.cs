using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using EcommerceAPI.Models.DTOs.User;
using System.Security.Cryptography;
using EcommerceAPI.Services.Security.Interfaces;
using AutoMapper;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.Auth.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;

namespace EcommerceAPI.Services.Auth
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
        private readonly PasswordHasher<UserEntity> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, ITokenGenerator tokenGenerator, IEmailService emailService, PasswordHasher<UserEntity> passwordHasher, IMapper mapper, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Logs in a user with the provided email and password.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        /// <returns>An authentication response containing a JWT token and user details.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when the email or password is incorrect.</exception>
        public async Task<AuthResponseDto> Login(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmail(loginDto.Email);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, loginDto.Password) == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }
            
            var token = _jwtService.GenerateJwtToken(_mapper.Map<UserGenerateTokenDto>(user));

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
            return new AuthResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(3),
                User = _mapper.Map<UserDto>(user)
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
            {
                _logger.LogWarning("User with email {Email} already exists", userRegister.Email);
                throw new InvalidOperationException("User with this email already exists");
            }

            userRegister.Password = _passwordHasher.HashPassword(new UserEntity(), userRegister.Password);

            string token = _tokenGenerator.GenerateToken();

            var newUser = _mapper.Map<UserEntity>(userRegister);
            newUser.EmailConfirmedToken = token;

            var userResult = await _userRepository.AddUser(newUser);

            if (userResult is null)
                throw new InvalidOperationException("Failed to register user");

            var emailResult = _emailService.SendVerificationEmail(userRegister.Email, token);

            if (!emailResult)
            {
                _logger.LogWarning("Failed to send verification email to {Email}", userRegister.Email);
                throw new InvalidOperationException("Failed to send verification email");
            }

            _logger.LogInformation("User {Email} registered successfully", userRegister.Email);
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
        public async Task<bool> ChangePassword(int id, UserChangePasswordDto changePassword)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, changePassword.OldPassword) == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Invalid password change attempt for user ID: {UserId}", id);
                throw new UnauthorizedAccessException("Invalid password");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, changePassword.NewPassword);

            var userResult = await _userRepository.UpdateUser(user);

            if (userResult is null)
                throw new InvalidOperationException("Failed to change password");

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", id);
            return true;
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

            int token = _tokenGenerator.Generate6DigitToken();

            user.ResetPasswordCode = token;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            var userResult = await _userRepository.UpdateUser(user);

            if (userResult is null)
                throw new InvalidOperationException("Failed to generate reset token");

            var emailResult = _emailService.SendForgotPassword(email, token);

            return emailResult;
        }

        /// <summary>
        /// Verifies the reset code and generates a new token for password reset.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="code">Reset code.</param>
        /// <returns>A new reset token.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> VerifyResetCode(UserVerifyResetCodeDto verifyResetCodeDto)
        {
            var user = await _userRepository.GetUserByEmail(verifyResetCodeDto.Email);

            if (user is null)
                throw new InvalidOperationException("User not found");

            if (user.ResetTokenExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Expired code");

            if (!user.ResetPasswordCode.HasValue || !CryptographicOperations.FixedTimeEquals(BitConverter.GetBytes(user.ResetPasswordCode.Value), BitConverter.GetBytes(verifyResetCodeDto.Code)))
            {
                throw new InvalidOperationException("Invalid code");
            }

            user.ResetPasswordCode = null;
            user.ResetTokenExpiresAt = null;
            await _userRepository.UpdateUser(user);

            return true;
        }

        ///    <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="code">Reset password token.</param>
        /// <param name="password">New password.</param>
        /// <returns>True if the password is reset successfully, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the token is invalid, expired, or reset fails.</exception>
        public async Task<bool> ResetPassword(UserResetPasswordDto userResetPassword)
        {
            var user = await _userRepository.GetUserByEmail(userResetPassword.Email);

            _logger.LogInformation("Resetting password for user {Email}", userResetPassword.Email);
            if (user is null || user.ResetPasswordCode != userResetPassword.ResetCode || user.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired reset token for user {Email}", userResetPassword.Email);
                throw new InvalidOperationException("Invalid or expired token");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, userResetPassword.NewPassword);

            user.ResetPasswordCode = null;
            user.ResetTokenExpiresAt = null;
            var userResult = await _userRepository.UpdateUser(user);

            if (userResult is null)
                throw new InvalidOperationException("Failed to reset password");

            return true;
        }
    }
}
