using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
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
    /// Unit tests for the AuthService ChangePassword method.
    /// </summary>
    public class ChangePassworTest
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
        /// Initializes a new instance of the <see cref="ChangePassworTest"/> class.
        /// </summary>
        public ChangePassworTest()
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
        /// Changes the password with valid data returns true.
        /// </summary>
        [Fact]
        public async Task ChangePassword_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var fakeHash = "hashed-old-password";
            var changePasswordDto = _fixture.Create<UserChangePasswordDto>();
            var user = CreateUserEntity(userId, fakeHash, UserProvider.local);

            _mockUserRepository.Setup(repo => repo.GetUserById(userId))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash!, changePasswordDto.OldPassword))
                .Returns(PasswordVerificationResult.Success);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash!, changePasswordDto.NewPassword))
                .Returns(PasswordVerificationResult.Failed);

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, changePasswordDto.NewPassword))
                .Returns("hashed-new-password");

            _mockUserRepository.Setup(repo => repo.UpdateUser(user))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ChangePassword(userId, changePasswordDto);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(user, changePasswordDto.NewPassword), Times.Once);
            _mockUserRepository.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        /// <summary>
        /// Changes the password when user not found throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ChangePassword_WhenUserNotFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var changePasswordDto = _fixture.Create<UserChangePasswordDto>();

            _mockUserRepository.Setup(repo => repo.GetUserById(userId))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ChangePassword(userId, changePasswordDto));
            _mockUserRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), changePasswordDto.NewPassword), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Changes the password when user is not local throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ChangePassword_WhenUserIsNotLocal_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var fakeHash = "hashed-old-password";
            var changePasswordDto = _fixture.Create<UserChangePasswordDto>();
            var user = CreateUserEntity(userId, fakeHash, UserProvider.google);

            _mockUserRepository.Setup(repo => repo.GetUserById(userId))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ChangePassword(userId, changePasswordDto));
            _mockUserRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), changePasswordDto.NewPassword), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Changes the password when old password is incorrect throws unauthorized access exception.
        /// </summary>
        [Fact]
        public async Task ChangePassword_WhenOldPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var userId = 1;
            var fakeHash = "hashed-old-password";
            var changePasswordDto = _fixture.Create<UserChangePasswordDto>();
            var user = CreateUserEntity(userId, fakeHash, UserProvider.local);

            _mockUserRepository.Setup(repo => repo.GetUserById(userId))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash!, changePasswordDto.OldPassword))
                .Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.ChangePassword(userId, changePasswordDto));
            _mockUserRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), changePasswordDto.NewPassword), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        /// <summary>
        /// Changes the password when new password is same as the old password throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task ChangePassword_WhenNewPasswordIsSameAsTheOldPassword_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var fakeHash = "hashed-old-password";
            var changePasswordDto = _fixture.Create<UserChangePasswordDto>();
            var user = CreateUserEntity(userId, fakeHash, UserProvider.local);

            _mockUserRepository.Setup(repo => repo.GetUserById(userId))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash!, changePasswordDto.OldPassword))
                .Returns(PasswordVerificationResult.Success);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash!, changePasswordDto.NewPassword))
                .Returns(PasswordVerificationResult.Success);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.ChangePassword(userId, changePasswordDto));
            _mockUserRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), changePasswordDto.NewPassword), Times.Never);
            _mockUserRepository.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        private UserEntity CreateUserEntity(int userId, string fakeHash, UserProvider provider)
        {
            var user = _fixture.Build<UserEntity>()
                .With(u => u.Id, userId)
                .With(u => u.Provider, provider)
                .With(u => u.PasswordHash, fakeHash)
                .Create();

            return user;
        }
    }
}