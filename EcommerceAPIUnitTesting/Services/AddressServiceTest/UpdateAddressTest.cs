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
    /// Unit tests for the UpdateAddress method in the AddressService class.
    /// </summary>
    public class UpdateAddressTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAddressTest"/> class.
        /// </summary>
        public UpdateAddressTest()
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
        /// Updates the address address not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task UpdateAddress_AddressNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var addressUpdateDto = _fixture.Create<AddressUpdateDto>();

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _addressService.UpdateAddress(userId, addressUpdateDto));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map(It.IsAny<AddressUpdateDto>(), It.IsAny<AddressEntity>()), Times.Never);
            _mockAddressRepository.Verify(sp => sp.UpdateAddress(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the address user not owner throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task UpdateAddress_UserNotOwner_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var addressUpdateDto = _fixture.Create<AddressUpdateDto>();

            var addressEntity = CreateAddressEntity(userId + 1);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.UpdateAddress(userId, addressUpdateDto));
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map(It.IsAny<AddressUpdateDto>(), It.IsAny<AddressEntity>()), Times.Never);
            _mockAddressRepository.Verify(sp => sp.UpdateAddress(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
        }

        /// <summary>
        /// Updates the address valid request updates address and invalidates cache.
        /// </summary>
        [Fact]
        public async Task UpdateAddress_ValidRequest_UpdatesAddressAndInvalidatesCache()
        {
            // Arrange 
            var userId = 1;
            var addressUpdateDto = _fixture.Create<AddressUpdateDto>();

            var addressEntity = CreateAddressEntity(userId);
            var addressDto = CreateAddressDto(userId);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            _mockMapper.Setup(sp => sp.Map(It.IsAny<AddressUpdateDto>(), It.IsAny<AddressEntity>()))
                .Callback<AddressUpdateDto, AddressEntity>((src, dest) =>
                {
                    dest.StreetAddress = src.StreetAddress;
                    dest.AddressLine2 = src.AddressLine2;
                    dest.City = src.City;
                    dest.State = src.State;
                    dest.PostalCode = src.PostalCode;
                    dest.Country = src.Country;
                });

            _mockAddressRepository.Setup(sp => sp.UpdateAddress(It.IsAny<AddressEntity>()))
                .ReturnsAsync(addressEntity);

            _mockCacheService.Setup(sp => sp.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()))
                .Returns(addressDto);

            // Act
            var result = await _addressService.UpdateAddress(userId, addressUpdateDto);

            // Assert
            Assert.NotNull(result);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map(It.IsAny<AddressUpdateDto>(), It.IsAny<AddressEntity>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.UpdateAddress(It.IsAny<AddressEntity>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Exactly(2));
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