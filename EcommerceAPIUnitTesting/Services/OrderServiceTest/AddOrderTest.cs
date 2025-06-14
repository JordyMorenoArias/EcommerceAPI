using AutoFixture;
using AutoMapper;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Order;
using Moq;
using Shouldly;

namespace EcommerceAPIUnitTesting.Services.OrderServiceTest
{
    /// <summary>
    /// Unit tests for the AddOrder method in the OrderService class.
    /// </summary>
    public class AddOrderTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddOrderTest"/> class.
        /// </summary>
        public AddOrderTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _addressRepositoryMock = new Mock<IAddressRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _mapperMock = new Mock<IMapper>();
            _fixture = new Fixture();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _addressRepositoryMock.Object,
                _cacheServiceMock.Object,
                _mapperMock.Object
            );
        }

        /// <summary>
        /// Adds the order throws invalid operation exception when user has no default address.
        /// </summary>
        [Fact]
        public async Task AddOrder_ThrowsInvalidOperationException_WhenUserHasNoDefaultAddress()
        {
            // Arrange
            var userId = 1;

            _addressRepositoryMock.Setup(a => a.GetDefaultAddressForUserAsync(userId))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () => await _orderService.AddOrder(userId));
            _addressRepositoryMock.Verify(a => a.GetDefaultAddressForUserAsync(userId), Times.Once);
            _orderRepositoryMock.Verify(o => o.AddOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Adds the order returns created order when default address exists.
        /// </summary>
        [Fact]
        public async Task AddOrder_ReturnsCreatedOrder_WhenDefaultAddressExists()
        {
            // Arrange
            var userId = 1;
            var addressEntity = CreateAddressEntity(userId);
            var orderEntity = CreateOrderEntity(userId, addressEntity.Id);
            var orderDto = CreateOrderDto(userId, addressEntity.Id);

            _addressRepositoryMock.Setup(a => a.GetDefaultAddressForUserAsync(userId))
                .ReturnsAsync(addressEntity);

            _orderRepositoryMock.Setup(o => o.AddOrder(It.IsAny<OrderEntity>()))
                .ReturnsAsync(orderEntity);

            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()))
                .Returns(orderDto);

            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.AddOrder(userId);

            // Assert
            result.ShouldNotBeNull();
            _addressRepositoryMock.Verify(a => a.GetDefaultAddressForUserAsync(userId), Times.Once);
            _orderRepositoryMock.Verify(o => o.AddOrder(It.IsAny<OrderEntity>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        private AddressEntity CreateAddressEntity(int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(a => a.UserId, userId)
                .Without(a => a.User)
                .Create();
        }

        private OrderEntity CreateOrderEntity(int userId, int AddressId)
        {
            return _fixture.Build<OrderEntity>()
                .With(o => o.UserId, userId)
                .With(o => o.ShippingAddressId, AddressId)
                .Without(o => o.ShippingAddress)
                .Without(o => o.OrderDetails)
                .Create();
        }

        private OrderDto CreateOrderDto(int userId, int AddressId)
        {
            return _fixture.Build<OrderDto>()
                .With(o => o.UserId, userId)
                .With(o => o.ShippingAddressId, AddressId)
                .Without(o => o.ShippingAddress)
                .Without(o => o.OrderDetails)
                .Create();
        }
    }
}