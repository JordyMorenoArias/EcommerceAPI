using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPIUnitTesting.Services.AddressServiceTest
{
    /// <summary>
    /// Unit tests for the SetDefaultAddress method in the AddressService class.
    /// </summary>
    public class SetDefaultAddressTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetDefaultAddressTest"/> class.
        /// </summary>
        public SetDefaultAddressTest()
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
        /// Sets the default address address not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task SetDefaultAddress_AddressNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _addressService.SetDefaultAddress(userId, addressId));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Sets the default address user not owner throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task SetDefaultAddress_UserNotOwner_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;
            var addressEntity = CreateAddressEntity(userId + 1);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.SetDefaultAddress(userId, addressId));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Sets the default address update fails throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task SetDefaultAddress_UpdateFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;
            var addressEntity = CreateAddressEntity(userId);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            _mockAddressRepository.Setup(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.SetDefaultAddress(userId, addressId));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Sets the default address valid request sets default and invalidates cache.
        /// </summary>
        [Fact]
        public async Task SetDefaultAddress_ValidRequest_SetsDefaultAndInvalidatesCache()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;
            var addressEntity = CreateAddressEntity(userId);

            var updatedAddressEntity = CreateAddressEntity(userId);

            var addressDto = CreateAddressDto(userId);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            _mockAddressRepository.Setup(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            _mockCacheService.Setup(sp => sp.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()))
                .Returns(addressDto);

            // Act
            var result = await _addressService.SetDefaultAddress(userId, addressId);

            // Assert
            Assert.NotNull(result);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.SetDefaultAddress(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Exactly(3));
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Once);
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