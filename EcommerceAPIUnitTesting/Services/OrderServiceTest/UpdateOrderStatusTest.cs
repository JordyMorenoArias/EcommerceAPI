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
    /// Unit tests for the <see cref="OrderService"/> method <see cref="OrderService.UpdateOrderStatus(int, UserRole, int, OrderStatus)"/>.
    /// </summary>
    public class UpdateOrderStatusTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOrderStatusTest"/> class.
        /// </summary>
        public UpdateOrderStatusTest()
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
        /// Updates the order status updates status and returns updated order.
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatus_UpdatesStatusAndReturnsUpdatedOrder()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;
            var newStatus = OrderStatus.Shipped;
            var orderEntity = CreateOrderEntity(userId, 1);
            var orderDto = CreateOrderDto(userId, 1, newStatus);

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            _orderRepositoryMock.Setup(r => r.UpdateOrder(It.IsAny<OrderEntity>()))
                .ReturnsAsync(orderEntity);

            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()))
                .Returns(orderDto);

            _cacheServiceMock.Setup(c => c.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.UpdateOrderStatus(orderId, role, userId, newStatus);

            // Assert
            result.ShouldNotBeNull();
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.UpdateOrder(It.IsAny<OrderEntity>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        /// <summary>
        /// Updates the order status throws key not found exception when order not found.
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatus_ThrowsKeyNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;
            var newStatus = OrderStatus.Shipped;

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync((OrderEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () => await _orderService.UpdateOrderStatus(orderId, role, userId, newStatus));
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Updates the order status throws unauthorized access exception when order does not belong to user.
        /// </summary>
        [Fact]
        public async Task UpdateOrderStatus_ThrowsUnauthorizedAccessException_WhenOrderDoesNotBelongToUser()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;
            var newStatus = OrderStatus.Shipped;
            var orderEntity = CreateOrderEntity(2, 1);

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () => await _orderService.UpdateOrderStatus(orderId, role, userId, newStatus));
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.UpdateOrder(It.IsAny<OrderEntity>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
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