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
    /// Unit tests for the GetOrderById method in the OrderService class.
    /// </summary>
    public class GetOrderByIdTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOrderByIdTest"/> class.
        /// </summary>
        public GetOrderByIdTest()
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
        /// Gets the order by identifier returns order when cached order exists.
        /// </summary>
        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenCachedOrderExists()
        {
            // Arrange
            var orderId = _fixture.Create<int>();
            var orderDto = CreateOrderDto(orderId);

            _cacheServiceMock.Setup(c => c.Get<OrderDto>(It.IsAny<string>()))
                .ReturnsAsync(orderDto);

            // Act
            var result = await _orderService.GetOrderById(orderId);

            // Assert
            result.ShouldNotBeNull();
            _cacheServiceMock.Verify(c => c.Get<OrderDto>(It.IsAny<string>()), Times.Once);
            _orderRepositoryMock.Verify(r => r.GetOrderById(It.IsAny<int>()), Times.Never);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), orderDto, It.IsAny<TimeSpan>()), Times.Never);
        }

        /// <summary>
        /// Gets the order by identifier returns order when order exists in repository.
        /// </summary>
        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenOrderExistsInRepository()
        {
            // Arrange
            var orderId = _fixture.Create<int>();
            var orderEntity = CreateOrderEntity(orderId);
            var orderDto = CreateOrderDto(orderId);

            _cacheServiceMock.Setup(c => c.Get<OrderDto>(It.IsAny<string>()))
                .ReturnsAsync((OrderDto?)null);

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(orderEntity);

            _mapperMock.Setup(m => m.Map<OrderDto>(orderEntity))
                .Returns(orderDto);

            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), orderDto, It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.GetOrderById(orderId);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(orderId);
            _cacheServiceMock.Verify(c => c.Get<OrderDto>(It.IsAny<string>()), Times.Once);
            _orderRepositoryMock.Verify(r => r.GetOrderById(It.IsAny<int>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), orderDto, It.IsAny<TimeSpan>()), Times.Once);
        }

        /// <summary>
        /// Gets the order by identifier throws key not found exception when order does not exist.
        /// </summary>
        [Fact]
        public async Task GetOrderById_ThrowsKeyNotFoundException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = _fixture.Create<int>();

            _cacheServiceMock.Setup(c => c.Get<OrderDto>(It.IsAny<string>()))
                .ReturnsAsync((OrderDto?)null);

            _orderRepositoryMock.Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync((OrderEntity?)null);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () => await _orderService.GetOrderById(orderId));
            _cacheServiceMock.Verify(c => c.Get<OrderDto>(It.IsAny<string>()), Times.Once);
            _orderRepositoryMock.Verify(r => r.GetOrderById(It.IsAny<int>()), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<OrderEntity>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<OrderDto>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        private OrderDto CreateOrderDto(int id = 1)
        {
            return _fixture.Build<OrderDto>()
                .With(o => o.Id, id)
                .Without(o => o.ShippingAddress)
                .Without(o => o.User)
                .Without(o => o.OrderDetails)
                .Create();
        }

        private OrderEntity CreateOrderEntity(int id = 1)
        {
            return _fixture.Build<OrderEntity>()
                .With(o => o.Id, id)
                .Without(o => o.ShippingAddress)
                .Without(o => o.User)
                .Without(o => o.OrderDetails)
                .Create();
        }
    }
}