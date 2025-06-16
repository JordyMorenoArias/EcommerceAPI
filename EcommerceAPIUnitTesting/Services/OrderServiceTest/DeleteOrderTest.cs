using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Order;
using Moq;
using Shouldly;

namespace EcommerceAPIUnitTesting.Services.OrderServiceTest
{
    /// <summary>
    /// Unit tests for the <see cref="OrderService"/> method <see cref="OrderService.DeleteOrder(int, UserRole, int)"/>.
    /// </summary>
    public class DeleteOrderTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteOrderTest"/> class.
        /// </summary>
        public DeleteOrderTest()
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
        /// Deletes the order deletes order and returns true.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_DeletesOrderAndReturnsTrue()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;
            var orderEntity = CreateOrderEntity(userId);

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);
            _orderRepositoryMock.Setup(r => r.DeleteOrder(orderEntity))
                .ReturnsAsync(true);

            // Act
            var result = await _orderService.DeleteOrder(userId, role, orderId);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Exactly(3));
            _orderRepositoryMock.Verify(r => r.DeleteOrder(orderEntity), Times.Once);
        }

        /// <summary>
        /// Deletes the order throws key not found exception when order not found.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_ThrowsKeyNotFoundException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync((OrderEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
                await _orderService.DeleteOrder(userId, role, orderId));
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.DeleteOrder(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the order throws unauthorized access exception when user is not authorized.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_ThrowsUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var orderId = 1;
            var userId = 1; // Different user ID
            var role = UserRole.Customer;
            var orderEntity = CreateOrderEntity(2); // Order belongs to user with ID 1

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
                await _orderService.DeleteOrder(userId, role, orderId));
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.DeleteOrder(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Deletes the order throws invalid operation exception when order does not have valid status.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_ThrowsInvalidOperationException_WhenOrderDoesNotHaveValidStatus()
        {
            // Arrange
            var orderId = 1;
            var userId = 1;
            var role = UserRole.Customer;
            var orderEntity = CreateOrderEntity(userId, OrderStatus.Paid); // Invalid status for deletion

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
                await _orderService.DeleteOrder(userId, role, orderId));
            _orderRepositoryMock.Verify(r => r.GetOrderById(orderId), Times.Once);
            _orderRepositoryMock.Verify(r => r.DeleteOrder(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
        }

        private OrderEntity CreateOrderEntity(int userId, OrderStatus status = OrderStatus.Draft)
        {
            return _fixture.Build<OrderEntity>()
                .With(o => o.UserId, userId)
                .With(o => o.Status, status)
                .Without(o => o.ShippingAddress)
                .Without(o => o.OrderDetails)
                .Create();
        }
    }
}