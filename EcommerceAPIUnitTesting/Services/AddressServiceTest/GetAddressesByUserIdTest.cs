using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPIUnitTesting.Services.AddressServiceTest
{
    /// <summary>
    /// Unit tests for the AddressService class, specifically for the GetAddressesByUserId method.
    /// </summary>
    public class GetAddressesByUserIdTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAddressesByUserIdTest"/> class.
        /// </summary>
        public GetAddressesByUserIdTest()
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
        /// Gets the addresses by user identifier cached addresses exist returns cached addresses.
        /// </summary>
        [Fact]
        public async Task GetAddressesByUserId_CachedAddressesExist_ReturnsCachedAddresses()
        {
            // Arrange
            var userId = 1;

            var cachedAddressesDto = CreateAddressesDto(userId);

            _mockCacheService.Setup(sp => sp.Get<IEnumerable<AddressDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedAddressesDto);

            // Act
            var result = await _addressService.GetAddressesByUserId(userId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<IEnumerable<AddressDto>>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressesByUserId(It.IsAny<int>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<AddressDto>>(It.IsAny<IEnumerable<AddressEntity>>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<IEnumerable<AddressDto>>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the addresses by user identifier addresses exist returns addresses and stores in cache.
        /// </summary>
        [Fact]
        public async Task GetAddressesByUserId_AddressesExist_ReturnsAddressesAndStoresInCache()
        {
            var userId = 1;

            var addressesEntity = CreateAddressesEntity(userId);
            var addressesDto = CreateAddressesDto(userId);

            _mockCacheService.Setup(sp => sp.Get<IEnumerable<AddressDto>>(It.IsAny<string>()))
                .ReturnsAsync((IEnumerable<AddressDto>?)null);

            _mockAddressRepository.Setup(sp => sp.GetAddressesByUserId(userId))
                .ReturnsAsync(addressesEntity);

            _mockMapper.Setup(sp => sp.Map<IEnumerable<AddressDto>>(addressesEntity))
                .Returns(addressesDto);

            _mockCacheService.Setup(sp => sp.Set(It.IsAny<string>(), addressesDto, It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _addressService.GetAddressesByUserId(userId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<IEnumerable<AddressDto>>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressesByUserId(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<IEnumerable<AddressDto>>(It.IsAny<IEnumerable<AddressEntity>>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<IEnumerable<AddressDto>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        private IEnumerable<AddressEntity> CreateAddressesEntity(int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(a => a.UserId, userId)
                .CreateMany(5)
                .ToList();
        }

        private IEnumerable<AddressDto> CreateAddressesDto(int userId)
        {
            return _fixture.Build<AddressDto>()
                .With(a => a.UserId, userId)
                .CreateMany(5)
                .ToList();
        }
    }
}