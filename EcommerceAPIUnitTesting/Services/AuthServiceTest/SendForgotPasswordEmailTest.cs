using AutoFixture;
using AutoMapper;
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
    /// Unit tests for the AuthService SendForgotPasswordEmail method.
    /// </summary>
    public class SendForgotPasswordEmailTest
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
        /// Initializes a new instance of the <see cref="SendForgotPasswordEmailTest"/> class.
        /// </summary>
        public SendForgotPasswordEmailTest()
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
        /// Sends the forgot password email with valid email returns true.
        /// </summary>
        [Fact]
        public async Task SendForgotPasswordEmail_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            var email = _fixture.Create<string>();

            var user = CreateUserEntity(email);
            var sixDigitCode = "123456";

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync(user);

            _mockTokenGenerator.Setup(generator => generator.Generate6DigitToken())
                .Returns(sixDigitCode);

            _mockUserRepository.Setup(repo => repo.UpdateUser(user))
                .ReturnsAsync(user);

            _mockEmailService.Setup(service => service.SendForgotPassword(email, sixDigitCode))
                .Verifiable();

            // Act
            var result = await _authService.SendForgotPasswordEmail(email);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockTokenGenerator.Verify(generator => generator.Generate6DigitToken(), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(user), Times.Once);
            _mockEmailService.Verify(service => service.SendForgotPassword(email, sixDigitCode), Times.Once);
        }

        /// <summary>
        /// Sends the forgot password email with nonexistent email throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task SendForgotPasswordEmail_WithNonexistentEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = _fixture.Create<string>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.SendForgotPasswordEmail(email));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockTokenGenerator.Verify(generator => generator.Generate6DigitToken(), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
            _mockEmailService.Verify(service => service.SendForgotPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private UserEntity CreateUserEntity(string email)
        {
            var user = _fixture.Build<UserEntity>()
                .With(u => u.Email, email)
                .Create();

            return user;
        }
    }
}