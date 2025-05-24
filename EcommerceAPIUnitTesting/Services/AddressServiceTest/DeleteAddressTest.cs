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
    /// Unit tests for the DeleteAddress method in the AddressService class.
    /// </summary>
    public class DeleteAddressTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAddressTest"/> class.
        /// </summary>
        public DeleteAddressTest()
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
        /// Deletes the address address not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task DeleteAddress_AddressNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;

            var addressEntity = CreateAddressEntity(userId);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(addressId))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _addressService.DeleteAddress(userId, addressId));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Once);
            _mockAddressRepository.Verify(sp => sp.DeleteAddress(addressId), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Deletes the address user not owner throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task DeleteAddress_UserNotOwner_ThrowsInvalidOperationException()
        {
            //Arrange
            var userId = 1;
            var addressId = 1;
            var addressEntity = CreateAddressEntity(userId + 1);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(addressId))
                .ReturnsAsync(addressEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.DeleteAddress(userId, addressId));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Once);
            _mockAddressRepository.Verify(sp => sp.DeleteAddress(addressId), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the address valid request deletes address and invalidates cache.
        /// </summary>
        [Fact]  
        public async Task DeleteAddress_ValidRequest_DeletesAddressAndInvalidatesCache()
        {
            // Arrange
            var userId = 1;
            var addressId = 1;

            var addressEntity = CreateAddressEntity(userId);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(addressId))
                .ReturnsAsync(addressEntity);

            _mockAddressRepository.Setup(sp => sp.DeleteAddress(addressId))
                .ReturnsAsync(true);

            // Act
            var result = await _addressService.DeleteAddress(userId, addressId);

            // Assert
            Assert.True(result);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Once);
            _mockAddressRepository.Verify(sp => sp.DeleteAddress(addressId), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Exactly(2));
        }

        private AddressEntity CreateAddressEntity(int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(x => x.UserId, userId)
                .Create();
        }
    }
}
