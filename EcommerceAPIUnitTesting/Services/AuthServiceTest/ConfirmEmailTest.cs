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
    /// Unit tests for the AuthService ConfirmEmail method.
    /// </summary>
    public class ConfirmEmailTest
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
        /// Initializes a new instance of the <see cref="ConfirmEmailTest"/> class.
        /// </summary>
        public ConfirmEmailTest()
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
        /// Confirms the email with valid token returns true.
        /// </summary>
        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsTrue()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var token = _fixture.Create<string>();
            var userEntity = CreateUserEntity(email, token);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync(userEntity);

            _mockUserRepository.Setup(repo => repo.UpdateUser(It.IsAny<UserEntity>()))
                .ReturnsAsync(userEntity);

            // Act
            var result = await _authService.ConfirmEmail(email, token);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Once);
        }

        /// <summary>
        /// Confirms the email with nonexistent user throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ConfirmEmail_WithNonexistentUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var token = _fixture.Create<string>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ConfirmEmail(email, token));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmEmail_WithAlreadyConfirmedEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var token = _fixture.Create<string>();
            var userEntity = CreateUserEntity(email, token, true);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync(userEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ConfirmEmail(email, token));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Confirms the email with invalid token throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ConfirmEmail_WithInvalidToken_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = _fixture.Create<string>();
            var token = _fixture.Create<string>();
            var userEntity = CreateUserEntity(email);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(email))
                .ReturnsAsync(userEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ConfirmEmail(email, token));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(email), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        private UserEntity CreateUserEntity(string email, string? token = null, bool isConfirmed = false)
        {
            return _fixture.Build<UserEntity>()
                .With(x => x.Email, email)
                .With(x => x.EmailConfirmedToken, token)
                .With(x => x.IsEmailConfirmed, isConfirmed)
                .Create();
        }
    }
}
