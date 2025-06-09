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
    /// Unit tests for the AuthService VerifyResetCode method.
    /// </summary>
    public class VerifyResetCodeTest
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
        /// Initializes a new instance of the <see cref="VerifyResetCodeTest"/> class.
        /// </summary>
        public VerifyResetCodeTest()
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
        /// Verifies the reset code with valid code returns true.
        /// </summary>
        [Fact]
        public async Task VerifyResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            var verifyResetCodeDto = _fixture.Create<UserVerifyResetCodeDto>();
            var user = CreateUserEntity(verifyResetCodeDto.Email, verifyResetCodeDto.Code, DateTime.UtcNow.AddHours(1));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(verifyResetCodeDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.VerifyResetCode(verifyResetCodeDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(verifyResetCodeDto.Email), Times.Once);
        }

        /// <summary>
        /// Verifies the reset code with non existent email throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task VerifyResetCode_WithNonExistentUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var verifyResetCodeDto = _fixture.Create<UserVerifyResetCodeDto>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(verifyResetCodeDto.Email))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.VerifyResetCode(verifyResetCodeDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(verifyResetCodeDto.Email), Times.Once);
        }

        /// <summary>
        /// Verifies the reset code with expired code throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task VerifyResetCode_WithExpiredCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var verifyResetCodeDto = _fixture.Create<UserVerifyResetCodeDto>();
            var user = CreateUserEntity(verifyResetCodeDto.Email, "expired_code", DateTime.UtcNow);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(verifyResetCodeDto.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.VerifyResetCode(verifyResetCodeDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(verifyResetCodeDto.Email), Times.Once);
        }

        /// <summary>
        /// Verifies the reset code with invalid code throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task VerifyResetCode_WithInvalidCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var verifyResetCodeDto = _fixture.Create<UserVerifyResetCodeDto>();
            var user = CreateUserEntity(verifyResetCodeDto.Email, "invalid_code", DateTime.UtcNow.AddHours(1));

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(verifyResetCodeDto.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.VerifyResetCode(verifyResetCodeDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(verifyResetCodeDto.Email), Times.Once);
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