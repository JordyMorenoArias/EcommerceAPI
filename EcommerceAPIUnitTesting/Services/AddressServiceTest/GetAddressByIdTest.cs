using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using Moq;

namespace EcommerceAPIUnitTesting.Services.AddressServiceTest
{
    /// <summary>
    /// Unit tests for the GetAddressById method in the AddressService class.
    /// </summary>
    public class GetAddressByIdTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAddressByIdTest"/> class.
        /// </summary>
        public GetAddressByIdTest()
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
        /// Gets the address by identifier cached address exists and authorized returns address dto.
        /// </summary>
        [Fact]
        public async Task GetAddressById_CachedAddressExistsAndAuthorized_ReturnsAddressDto()
        {
            // Arrange
            var userId = 1;
            var userRole = UserRole.Customer;
            var addressId = 1;

            var addressDto = CreateAddressDto(addressId, userId);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync(addressDto);

            // Act 
            var result = await _addressService.GetAddressById(userId, userRole, addressId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the address by identifier cached address exists and unauthorized throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task GetAddressById_CachedAddressExistsAndUnauthorized_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var userRole = UserRole.Customer;
            var addressId = 1;

            var addressDto = CreateAddressDto(addressId, 2);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync(addressDto);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.GetAddressById(userId, userRole, addressId));
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Never);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the address by identifier address not found throws key not found exception.
        /// </summary>
        [Fact]
        public async Task GetAddressById_AddressNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = 1;
            var userRole = UserRole.Customer;
            var addressId = 1;

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync((AddressDto?)null);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(addressId))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _addressService.GetAddressById(userId, userRole, addressId));
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Once);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the address by identifier address exists and unauthorized throws invalid operation exception.
        /// </summary>
        [Fact]
        public async Task GetAddressById_AddressExistsAndUnauthorized_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var userRole = UserRole.Customer;
            var addressId = 1;

            var addressEntity = CreateAddressEntity(addressId, 2);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync((AddressDto?)null);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(addressId))
                .ReturnsAsync(addressEntity);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _addressService.GetAddressById(userId, userRole, addressId));
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(addressId), Times.Once);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Never);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the address by identifier address exists and authorized returns address dto.
        /// </summary>
        [Fact]
        public async Task GetAddressById_AddressExistsAndAuthorized_ReturnsAddressDto()
        {
            // Arrange
            var userId = 1;
            var userRole = UserRole.Customer;
            var addressId = 1;

            var addressEntity = CreateAddressEntity(addressId, userId);
            var addressDto = CreateAddressDto(addressId, userId);

            _mockCacheService.Setup(sp => sp.Get<AddressDto>(It.IsAny<string>()))
                .ReturnsAsync((AddressDto?)null);

            _mockAddressRepository.Setup(sp => sp.GetAddressById(It.IsAny<int>()))
                .ReturnsAsync(addressEntity);

            _mockMapper.Setup(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()))
                .Returns(addressDto);

            _mockCacheService.Setup(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _addressService.GetAddressById(userId, userRole, addressId);

            // Assert
            Assert.NotNull(result);
            _mockCacheService.Verify(sp => sp.Get<AddressDto>(It.IsAny<string>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressById(It.IsAny<int>()), Times.Once);
            _mockMapper.Verify(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Set(It.IsAny<string>(), It.IsAny<AddressDto>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        private AddressEntity CreateAddressEntity(int id, int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(x => x.Id, id)
                .With(x => x.UserId, userId)
                .Create();
        }

        private AddressDto CreateAddressDto(int id, int userId)
        {
            return _fixture.Build<AddressDto>()
                .With(x => x.Id, id)
                .With(x => x.UserId, userId)
                .Create();
        }
    }
}
