using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Auth;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Security.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EcommerceAPIUnitTesting.Services.AuthServiceTest
{
    /// <summary>
    /// Unit tests for the AuthService ResetPassword method.
    /// </summary>
    public class ResetPasswordTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ITokenGenerator> _mockTokenGenerator;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IPasswordHasher<UserEntity>> _mockPasswordHasher;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordTest"/> class.
        /// </summary>
        public ResetPasswordTest()
        {
            // Initialize mocks
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _mockTokenGenerator = new Mock<ITokenGenerator>();
            _mockEmailService = new Mock<IEmailService>();
            _mockPasswordHasher = new Mock<IPasswordHasher<UserEntity>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            // Initialize AutoFixture
            _fixture = new Fixture();

            // Create an instance of AuthService with the mocked dependencies
            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockJwtService.Object,
                _mockTokenGenerator.Object,
                _mockEmailService.Object,
                _mockPasswordHasher.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        /// <summary>
        /// Resets the password with valid token returns true.
        /// </summary>
        [Fact]
        public async Task ResetPassword_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var userResetPassword = _fixture.Create<UserResetPasswordDto>();
            var user = CreateUserEntity(userResetPassword.Email, userResetPassword.ResetCode, DateTime.UtcNow.AddHours(1));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userResetPassword.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), userResetPassword.NewPassword))
                .Returns("hashedPassword");

            _mockUserRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserEntity>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ResetPassword(userResetPassword);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userResetPassword.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), userResetPassword.NewPassword), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Once);
        }

        /// <summary>
        /// Resets the password with non existent user throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ResetPassword_WithNonExistentUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var userResetPassword = _fixture.Create<UserResetPasswordDto>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userResetPassword.Email))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ResetPassword(userResetPassword));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userResetPassword.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Resets the password with expired token throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ResetPassword_WithExpiredToken_ThrowsInvalidOperationException()
        {
            // Arrange
            var userResetPassword = _fixture.Create<UserResetPasswordDto>();
            var user = CreateUserEntity(userResetPassword.Email, userResetPassword.ResetCode, DateTime.UtcNow.AddHours(-1));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userResetPassword.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ResetPassword(userResetPassword));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userResetPassword.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Resets the password with invalid reset code throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ResetPassword_WithInvalidResetCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var userResetPassword = _fixture.Create<UserResetPasswordDto>();
            var user = CreateUserEntity(userResetPassword.Email, "invalidCode", DateTime.UtcNow.AddHours(1));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userResetPassword.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ResetPassword(userResetPassword));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userResetPassword.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        private UserEntity CreateUserEntity(string email, string resetCode, DateTime Expiration)
        {
            var user = _fixture.Build<UserEntity>()
                .With(u => u.Email, email)
                .With(u => u.ResetPasswordCode, resetCode)
                .With(u => u.ResetTokenExpiresAt, Expiration)
                .Create();

            return user;
        }
    }
}