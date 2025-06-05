using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.Auth;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Auth;
using EcommerceAPI.Services.Infrastructure;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Security.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EcommerceAPIUnitTesting.Services.AuthServiceTest
{
    public class LoginTest
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

        public LoginTest()
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

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var loginDto = _fixture.Create<UserLoginDto>();
            var userEntity = CreateUserEntity(loginDto.Email);
            var userDto = CreateUserDto(loginDto.Email);
            var userTokenDto = CreateUserTokenDto(loginDto.Email);
            var token = _fixture.Create<string>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(loginDto.Email))
                .ReturnsAsync(userEntity);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash!, loginDto.Password))
                .Returns(PasswordVerificationResult.Success);

            _mockMapper.Setup(map => map.Map<UserGenerateTokenDto>(userEntity))
                .Returns(userTokenDto);

            _mockJwtService.Setup(jwt => jwt.GenerateJwtToken(userTokenDto))
                .Returns(token);

            _mockMapper.Setup(map => map.Map<UserDto>(userEntity))
                .Returns(userDto);

            // Act
            var result = await _authService.Login(loginDto);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(loginDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash!, loginDto.Password), Times.Once);
            _mockMapper.Verify(map => map.Map<UserGenerateTokenDto>(userEntity), Times.Once);
            _mockJwtService.Verify(jwt => jwt.GenerateJwtToken(userTokenDto), Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = _fixture.Create<UserLoginDto>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(loginDto.Email))
                .ReturnsAsync((UserEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(loginDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(It.IsAny<UserEntity>(), It.IsAny<string>(), loginDto.Password), Times.Never);
            _mockMapper.Verify(map => map.Map<UserGenerateTokenDto>(It.IsAny<UserEntity>()), Times.Never);
            _mockJwtService.Verify(jwt => jwt.GenerateJwtToken(It.IsAny<UserGenerateTokenDto>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithNullPasswordHash_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = _fixture.Create<UserLoginDto>();

            var userEntity = CreateUserEntity(loginDto.Email);
            userEntity.PasswordHash = null; // Simulate null password hash

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(loginDto.Email))
                .ReturnsAsync(userEntity);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(loginDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(userEntity, It.IsAny<string>(), loginDto.Password), Times.Never);
            _mockMapper.Verify(map => map.Map<UserGenerateTokenDto>(It.IsAny<UserEntity>()), Times.Never);
            _mockJwtService.Verify(jwt => jwt.GenerateJwtToken(It.IsAny<UserGenerateTokenDto>()), Times.Never);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = _fixture.Create<UserLoginDto>();
            var userEntity = CreateUserEntity(loginDto.Email);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(loginDto.Email))
                .ReturnsAsync(userEntity);

            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash!, loginDto.Password))
                .Returns(PasswordVerificationResult.Failed);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(loginDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash!, loginDto.Password), Times.Once);
            _mockMapper.Verify(map => map.Map<UserGenerateTokenDto>(It.IsAny<UserEntity>()), Times.Never);
            _mockJwtService.Verify(jwt => jwt.GenerateJwtToken(It.IsAny<UserGenerateTokenDto>()), Times.Never);
        }

        private UserEntity CreateUserEntity(string email)
        {
            return _fixture.Build<UserEntity>()
                .With(x => x.Email, email)
                .Create();
        }

        private UserDto CreateUserDto(string email)
        {
            return _fixture.Build<UserDto>()
                .With(x => x.Email, email)
                .Create();
        }

        private UserGenerateTokenDto CreateUserTokenDto(string email)
        {
            return _fixture.Build<UserGenerateTokenDto>()
                .With(x => x.Email, email)
                .Create();
        }
    }
}