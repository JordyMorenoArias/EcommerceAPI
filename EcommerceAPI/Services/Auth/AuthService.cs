using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.Security.Interfaces;
using AutoMapper;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.Auth.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;

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
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="jwtService">The JWT service.</param>
        /// <param name="tokenGenerator">The token generator.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="passwordHasher">The password hasher.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public AuthService(IUserRepository userRepository, IJwtService jwtService, ITokenGenerator tokenGenerator, IEmailService emailService, IPasswordHasher<UserEntity> passwordHasher, IMapper mapper, ILogger<AuthService> logger)
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

            if (user == null)
            {
                _logger.LogWarning("Login attempt failed: user not found for email {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Login attempt failed: no password hash found for user {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login attempt failed: invalid password for user {Email}", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var userTokenDto = _mapper.Map<UserGenerateTokenDto>(user);
            var token = _jwtService.GenerateJwtToken(userTokenDto);

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);
           
            var userDto = _mapper.Map<UserDto>(user);
            return new AuthResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(3),
                User = userDto
            };
        }

        /// <summary>
        /// Registers the specified user register.
        /// </summary>
        /// <param name="userRegister">The user register.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the registration process.</returns>
        /// <exception cref="System.InvalidOperationException">User with this email already exists</exception>
        public async Task<OperationResult> Register(UserRegisterDto userRegister)
        {
            var user = await _userRepository.GetUserByEmail(userRegister.Email);

            if (user != null)
            {
                _logger.LogWarning("User with email {Email} already exists", userRegister.Email);
                throw new InvalidOperationException("User with this email already exists");
            }

            var passwordHash = _passwordHasher.HashPassword(new UserEntity(), userRegister.Password);
            string token = _tokenGenerator.GenerateToken();

            var userEntity = _mapper.Map<UserEntity>(userRegister);
            
            userEntity.Provider = UserProvider.local;
            userEntity.PasswordHash = passwordHash;
            userEntity.EmailConfirmedToken = token;
            var userResult = await _userRepository.AddUser(userEntity);

            _emailService.SendVerificationEmail(userResult.Email, token);

            _logger.LogInformation("User {Email} registered successfully", userResult.Email);
            return new OperationResult
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        /// <summary>
        /// Confirms the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="token">The token.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the email confirmation.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// User not found
        /// or
        /// Email already confirmed
        /// or
        /// Invalid token
        /// </exception>
        public async Task<OperationResult> ConfirmEmail(string email, string token)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
                throw new InvalidOperationException("User not found");

            if (user.IsEmailConfirmed)
                throw new InvalidOperationException("Email already confirmed");

            if (user.EmailConfirmedToken != token)
                throw new InvalidOperationException("Invalid token");

            user.IsEmailConfirmed = true;
            user.EmailConfirmedToken = string.Empty;
            await _userRepository.UpdateUser(user);

            return new OperationResult
            {
                Success = true,
                Message = "Email confirmed successfully"
            };
        }

        /// <summary>
        /// Changes a user's password after validating the old password.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="changePassword"></param>
        /// <returns>
        /// An <see cref="OperationResult"/> indicating whether the password change was successful,
        /// along with an optional message describing the result.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// User not found
        /// or
        /// Password change is only allowed for local users
        /// or
        /// Local user must have a password set.
        /// or
        /// New password cannot be the same as the old password
        /// </exception>
        /// <exception cref="System.UnauthorizedAccessException">Invalid old password</exception>
        public async Task<OperationResult> ChangePassword(int id, UserChangePasswordDto changePassword)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for password change", id);
                throw new InvalidOperationException("User not found");
            }

            if (user.Provider != UserProvider.local)
            {
                _logger.LogWarning("Password change attempted for non-local user ID: {UserId}", id);
                throw new InvalidOperationException("Password change is only allowed for local users");
            }

            if (user.PasswordHash is null)
            {
                throw new InvalidOperationException("Local user must have a password set.");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePassword.OldPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Invalid old password for user ID: {UserId}", id);
                throw new UnauthorizedAccessException("Invalid old password");
            }

            var newPasswordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePassword.NewPassword);
            if (newPasswordVerificationResult == PasswordVerificationResult.Success)
            {
                _logger.LogWarning("New password cannot be the same as the old password for user ID: {UserId}", id);
                throw new InvalidOperationException("New password cannot be the same as the old password");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, changePassword.NewPassword);

            var userResult = await _userRepository.UpdateUser(user);

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", id);
            return new OperationResult 
            { 
                Success = true, 
                Message = "Password changed successfully" 
            };
        }

        /// <summary>
        /// Sends a forgot password email to the user.
        /// </summary>
        /// <param name="email">The email address to send the forgot password email to.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a boolean value indicating if the email was successfully sent.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">User with this email does not exist</exception>
        public async Task<OperationResult> SendForgotPasswordEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if(user is null)
                throw new InvalidOperationException("User with this email does not exist");

            var sixDigitCode = _tokenGenerator.Generate6DigitToken();

            user.ResetPasswordCode = sixDigitCode;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateUser(user);

            _emailService.SendForgotPassword(email, sixDigitCode);

            return new OperationResult
            {
                Success = true,
                Message = "Reset password email sent successfully"
            };
        }

        /// <summary>
        /// Verifies the reset code.
        /// </summary>
        /// <param name="verifyResetCodeDto">The verify reset code dto.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating whether the reset code is valid.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// User not found
        /// or
        /// Expired code
        /// </exception>
        public async Task<OperationResult> VerifyResetCode(UserVerifyResetCodeDto verifyResetCodeDto)
        {
            var user = await _userRepository.GetUserByEmail(verifyResetCodeDto.Email);

            if (user is null)
                throw new InvalidOperationException("User not found");

            if (user.ResetTokenExpiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Expired code");

            if (user.ResetPasswordCode != verifyResetCodeDto.Code)
                throw new InvalidOperationException("Invalid reset code");

            return new OperationResult
            {
                Success = true,
                Message = "Reset code verified successfully"
            };
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="userResetPassword">The user reset password.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="OperationResult"/> indicating the result of the password reset operation.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Invalid or expired token
        /// or
        /// Failed to reset password
        /// </exception>
        public async Task<OperationResult> ResetPassword(UserResetPasswordDto userResetPassword)
        {
            var user = await _userRepository.GetUserByEmail(userResetPassword.Email);

            if (user is null)
            {
                _logger.LogWarning("Reset password attempt failed: user not found for email {Email}", userResetPassword.Email);
                throw new InvalidOperationException("User not found");
            }

            if (user.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Reset password attempt failed: expired reset token for user {Email}", userResetPassword.Email);
                throw new InvalidOperationException("Expired reset token");
            }

            if (user.ResetPasswordCode != userResetPassword.ResetCode)
            {
                _logger.LogWarning("Reset password attempt failed: invalid reset code for user {Email}", userResetPassword.Email);
                throw new InvalidOperationException("Invalid reset code");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, userResetPassword.NewPassword);
            user.ResetPasswordCode = null;
            user.ResetTokenExpiresAt = null;
            await _userRepository.UpdateUser(user);

            return new OperationResult
            {
                Success = true,
                Message = "Email confirmed successfully"
            };
        }
    }
}