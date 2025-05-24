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
    /// Unit tests for the AddAddress method in the AddressService class.
    /// </summary>
    public class AddAddressTest
    {
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AddressService _addressService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddAddressTest"/> class.
        /// </summary>
        public AddAddressTest()
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
        /// Adds the address first address sets as default and returns address dto.
        /// </summary>
        [Fact]
        public async Task AddAddress_FirstAddress_SetsAsDefaultAndReturnsAddressDto()
        {
            // Arrange
            int userId = 1;
            var addressAddDto = _fixture.Create<AddressAddDto>();

            var addressEntity = CreateAddressEntity(userId);
            var addressDto = CreateAddressDto(userId);

            _mockMapper.Setup(sp => sp.Map<AddressEntity>(It.IsAny<AddressAddDto>()))
                .Returns(addressEntity);

            _mockAddressRepository.Setup(sp => sp.GetAddressesByUserId(It.IsAny<int>()))
                .ReturnsAsync(Enumerable.Empty<AddressEntity>);

            _mockAddressRepository.Setup(sp => sp.AddAddress(It.IsAny<AddressEntity>()))
                .ReturnsAsync(addressEntity);

            _mockCacheService.Setup(sp => sp.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(sp => sp.Map<AddressDto>(It.IsAny<AddressEntity>()))
                .Returns(addressDto);

            // Act
            var result = await _addressService.AddAddress(userId, addressAddDto);

            // Assert
            Assert.NotNull(result);
            _mockMapper.Verify(sp => sp.Map<AddressEntity>(It.IsAny<AddressAddDto>()), Times.Once);
            _mockAddressRepository.Verify(sp => sp.GetAddressesByUserId(userId), Times.Once);
            _mockAddressRepository.Verify(sp => sp.AddAddress(It.IsAny<AddressEntity>()), Times.Once);
            _mockCacheService.Verify(sp => sp.Remove(It.IsAny<string>()), Times.Once);
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