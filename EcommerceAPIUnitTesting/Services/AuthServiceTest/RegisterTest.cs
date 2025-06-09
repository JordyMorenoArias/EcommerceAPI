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
    /// Unit tests for the AuthService Register method.
    /// </summary>
    public class RegisterTest
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
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <returns></returns>
        public RegisterTest()
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
        /// Registers the with valid data returns true.
        /// </summary>
        [Fact]
        public async Task Register_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userRegisterDto = _fixture.Create<UserRegisterDto>();
            var userEntity = CreateUserEntity(userRegisterDto.Email);
            var passwordHash = _fixture.Create<string>();
            var token = _fixture.Create<string>();

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userRegisterDto.Email))
                .ReturnsAsync((UserEntity?)null);

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()))
                .Returns(passwordHash);

            _mockTokenGenerator.Setup(tokenGen => tokenGen.GenerateToken())
                .Returns(token);

            _mockMapper.Setup(map => map.Map<UserEntity>(userRegisterDto))
                .Returns(userEntity);

            _mockUserRepository.Setup(repo => repo.AddUser(userEntity))
                .ReturnsAsync(userEntity);

            _mockEmailService.Setup(emailService => emailService.SendVerificationEmail(userRegisterDto.Email, token))
                .Returns(true);

            // Act
            var result = await _authService.Register(userRegisterDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userRegisterDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), userRegisterDto.Password), Times.Once);
            _mockTokenGenerator.Verify(tokenGen => tokenGen.GenerateToken(), Times.Once);
            _mockMapper.Verify(map => map.Map<UserEntity>(userRegisterDto), Times.Once);
            _mockUserRepository.Verify(repo => repo.AddUser(userEntity), Times.Once);
            _mockEmailService.Verify(emailService => emailService.SendVerificationEmail(userRegisterDto.Email, token), Times.Once);
        }

        /// <summary>
        /// Registers the with existing email throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task Register_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            var userRegisterDto = _fixture.Create<UserRegisterDto>();
            var existingUser = CreateUserEntity(userRegisterDto.Email);

            _mockUserRepository.Setup(repo => repo.GetUserByEmail(userRegisterDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.Register(userRegisterDto));
            _mockUserRepository.Verify(repo => repo.GetUserByEmail(userRegisterDto.Email), Times.Once);
            _mockPasswordHasher.Verify(hasher => hasher.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _mockTokenGenerator.Verify(tokenGen => tokenGen.GenerateToken(), Times.Never);
            _mockMapper.Verify(map => map.Map<UserEntity>(It.IsAny<UserRegisterDto>()), Times.Never);
            _mockUserRepository.Verify(repo => repo.AddUser(It.IsAny<UserEntity>()), Times.Never);
            _mockEmailService.Verify(emailService => emailService.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }


        private UserEntity CreateUserEntity(string email)
        {
            return _fixture.Build<UserEntity>()
                .With(x => x.Email, email)
                .Create();
        }
    }
}