using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.AddressServiceTest
{
    /// <summary>
    /// Unit tests for the GetDefaultAddressForUser method in the AddressService class.
    /// </summary>
    public class GetDefaultAddressForUser
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAddressByIdTest"/> class.
        /// </summary>
        public GetDefaultAddressForUser()
        {
            _mockAddressRepository = new Mock<IAddressRepository>();
            _mockCacheService = new Mock<ICacheService>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();

            _addressService = new AddressService(
                _mockAddressRepository.Object,
                _mockCacheService.Object,
                _mockMapper.Object
            );
        }

        /// <summary>
        /// Gets the default address for user cached address exists returns address dto.
        /// </summary>
        [Fact]
        public async Task GetDefaultAddressForUser_CachedAddressExists_ReturnsAddressDto()
        {
            // Arrange
            var userId = 1;

            var addressDto = CreateAddressDto(userId);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync(addressDto);

            // Act
            var result = await _addressService.GetDefaultAddressForUser(userId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetDefaultAddressForUserAsync(userId), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the default address for user address not found returns null.
        /// </summary>
        [Fact]
        public async Task GetDefaultAddressForUser_AddressNotFound_ReturnsNull()
        {
            // Arrange
            var userId = 1;

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync((AddressDto?)null);

            _mockAddressRepository.Setup(sp => sp.GetDefaultAddressForUserAsync(It.IsAny<int>()))
                .ReturnsAsync((AddressEntity?)null);

            // Act
            var result = await _addressService.GetDefaultAddressForUser(userId);

            // Assert
            Assert.Null(result);
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetDefaultAddressForUserAsync(userId), Times.Once);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the default address for user address exists returns address dto and stores in cache.
        /// </summary>
        [Fact]
        public async Task GetDefaultAddressForUser_AddressExists_ReturnsAddressDtoAndStoresInCache()
        {
            // Arrange
            var userId = 1;

            var addressEntity = CreateAddressEntity(userId);
            var addressDto = CreateAddressDto(userId);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync((AddressDto?)null);

            _mockAddressRepository.Setup(sp => sp.GetDefaultAddressForUserAsync(userId))
                .ReturnsAsync(addressEntity);

            _mockMapper.Setup(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()))
                .Returns(addressDto);

            _mockCacheService.Setup(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _addressService.GetDefaultAddressForUser(userId);

            // Assert
            Assert.NotNull(result);

            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetDefaultAddressForUserAsync(userId), Times.Once);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        private AddressEntity CreateAddressEntity(int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(x => x.UserId, userId)
                .Create();
        }

        private AddressDto CreateAddressDto(int userId)
        {
            return _fixture.Build<AddressDto>()
                .With(x => x.UserId, userId)
                .Create();
        }
    }
}