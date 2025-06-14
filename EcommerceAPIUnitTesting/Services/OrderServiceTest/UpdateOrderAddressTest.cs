using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
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
    /// Unit tests for the <see cref="OrderService.UpdateOrderAddress"/> method.
    /// </summary>
    public class UpdateOrderAddressTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAddressOrderTest"/> class.
        /// </summary>
        public UpdateOrderAddressTest()
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
        /// Updates the order address throws key not found exception when order does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateOrderAddress_ThrowsKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int orderId = 1;

            _orderRepositoryMock.Setup(o => o.GetOrderById(orderId))
                .ReturnsAsync((OrderEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrderAddress(userId, orderId, 1));
            _orderRepositoryMock.Verify(o => o.GetOrderById(orderId), Times.Once);
            _addressRepositoryMock.Verify(a => a.GetDefaultAddressForUserAsync(userId), Times.Never);
            _orderRepositoryMock.Verify(o => o.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Updates the order address throws unauthorized access exception when user identifier does not match order user identifier.
        /// </summary>
        [Fact]
        public async Task UpdateOrderAddress_ThrowsUnauthorizedAccessException_WhenUserIdDoesNotMatchOrderUserId()
        {
            // Arrange
            int userId = 1;
            int orderId = 1;
            var orderEntity = CreateOrderEntity(2, 1);

            _orderRepositoryMock.Setup(o => o.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () => await _orderService.UpdateOrderAddress(userId, orderId, 1));
            _orderRepositoryMock.Verify(o => o.GetOrderById(orderId), Times.Once);
            _addressRepositoryMock.Verify(a => a.GetDefaultAddressForUserAsync(userId), Times.Never);
            _orderRepositoryMock.Verify(o => o.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Updates the order address throws invalid operation exception when order status not is draft.
        /// </summary>
        [Fact]
        public async Task UpdateOrderAddress_ThrowsInvalidOperationException_WhenOrderStatusNotIsDraft()
        {
            // Arrange
            int userId = 1;
            int orderId = 1;
            var orderEntity = CreateOrderEntity(userId, 1, OrderStatus.Paid);

            _orderRepositoryMock.Setup(o => o.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () => await _orderService.UpdateOrderAddress(userId, orderId, 1));
            _orderRepositoryMock.Verify(o => o.GetOrderById(orderId), Times.Once);
            _addressRepositoryMock.Verify(a => a.GetDefaultAddressForUserAsync(userId), Times.Never);
            _orderRepositoryMock.Verify(o => o.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Updates the address order throws key not found exception when address does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateAddressOrder_ThrowsKeyNotFoundException_WhenAddressDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int orderId = 1;
            var orderEntity = CreateOrderEntity(userId, 1);

            _orderRepositoryMock.Setup(o => o.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            _addressRepositoryMock.Setup(a => a.GetAddressById(userId))
                .ReturnsAsync((AddressEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrderAddress(userId, orderId, 1));
            _orderRepositoryMock.Verify(o => o.GetOrderById(orderId), Times.Once);
            _addressRepositoryMock.Verify(a => a.GetAddressById(userId), Times.Once);
            _orderRepositoryMock.Verify(o => o.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Updates the address order updates shipping address and returns updated order.
        /// </summary>
        [Fact]
        public async Task UpdateAddressOrder_UpdatesShippingAddressAndReturnsUpdatedOrder()
        {
            // Arrange
            int userId = 1;
            int orderId = 1;
            int newAddressId = 2;

            var orderEntity = CreateOrderEntity(userId, 1);
            var addressEntity = CreateAddressEntity(newAddressId, userId);
            var updatedOrderEntity = CreateOrderEntity(userId, newAddressId);
            var updatedOrderDto = CreateOrderDto(userId, newAddressId);

            _orderRepositoryMock.Setup(o => o.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            _addressRepositoryMock.Setup(a => a.GetAddressById(newAddressId))
                .ReturnsAsync(addressEntity);

            _orderRepositoryMock.Setup(o => o.UpdateOrder(It.IsAny<OrderEntity>()))
                .ReturnsAsync(updatedOrderEntity);

            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()))
                .Returns(updatedOrderDto);

            _cacheServiceMock.Setup(c => c.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.UpdateOrderAddress(userId, orderId, newAddressId);

            // Assert
            result.ShouldNotBeNull();
            _orderRepositoryMock.Verify(o => o.GetOrderById(orderId), Times.Once);
            _addressRepositoryMock.Verify(a => a.GetAddressById(newAddressId), Times.Once);
            _orderRepositoryMock.Verify(o => o.UpdateOrder(It.IsAny<OrderEntity>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        private AddressEntity CreateAddressEntity(int addressId, int userId)
        {
            return _fixture.Build<AddressEntity>()
                .With(a => a.Id, addressId)
                .With(a => a.UserId, userId)
                .Without(a => a.User)
                .Create();
        }

        private OrderEntity CreateOrderEntity(int userId, int AddressId, OrderStatus status = OrderStatus.Draft)
        {
            return _fixture.Build<OrderEntity>()
                .With(o => o.UserId, userId)
                .With(o => o.ShippingAddressId, AddressId)
                .With(o => o.Status, status)
                .Without(o => o.ShippingAddress)
                .Without(o => o.OrderDetails)
                .Create();
        }

        private OrderDto CreateOrderDto(int userId, int AddressId, OrderStatus status = OrderStatus.Draft)
        {
            return _fixture.Build<OrderDto>()
                .With(o => o.UserId, userId)
                .With(o => o.ShippingAddressId, AddressId)
                .With(o => o.Status, status)
                .Without(o => o.ShippingAddress)
                .Without(o => o.OrderDetails)
                .Create();
        }
    }
}