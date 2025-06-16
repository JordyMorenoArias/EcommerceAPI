using AutoFixture;
using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
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
    /// Unit tests for the GetSellerOrders method in the OrderService class.
    /// </summary>
    public class GetSellerOrdersTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;
        private readonly Fixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSellerOrdersTest"/> class.
        /// </summary>
        public GetSellerOrdersTest()
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
        /// Gets the seller orders throws unauthorized access exception when user accesses other seller orders and is not admin.
        /// </summary>
        [Fact]
        public async Task GetSellerOrders_ThrowsUnauthorizedAccessException_WhenUserAccessesOtherSellerOrders_AndIsNotAdmin()
        {
            // Arrange
            int userId = 1;
            var role = UserRole.Seller;
            var parameters = CreateQueryParameters(2);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () => await _orderService.GetSellerOrders(userId, role, parameters));
            _cacheServiceMock.Verify(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()), Times.Never);
            _orderRepositoryMock.Verify(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()), Times.Never);
            _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PagedResult<OrderDto>>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the seller orders throws unauthorized access exception when user is not seller or admin.
        /// </summary>
        [Fact]
        public async Task GetSellerOrders_ThrowsUnauthorizedAccessException_WhenUserIsNotSellerOrAdmin()
        {
            // Arrange
            int userId = 1;
            var role = UserRole.Customer;
            var parameters = CreateQueryParameters(userId);

            // Act & Assert
            await Should.ThrowAsync<UnauthorizedAccessException>(async () => await _orderService.GetSellerOrders(userId, role, parameters));
            _cacheServiceMock.Verify(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()), Times.Never);
            _orderRepositoryMock.Verify(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()), Times.Never);
            _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PagedResult<OrderDto>>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the seller orders returns orders when cached orders exist.
        /// </summary>
        [Fact]
        public async Task GetSellerOrders_ReturnsOrders_WhenCachedOrdersExist()
        {
            // Arrange
            var userId = 1;
            var role = UserRole.Seller;
            var parameters = CreateQueryParameters(userId);
            var cachedOrdersDto = CreatePagedResultOrderDto(userId);

            _cacheServiceMock.Setup(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()))
                .ReturnsAsync(cachedOrdersDto);

            // Act 
            var result = await _orderService.GetSellerOrders(userId, role, parameters);

            // Assert
            result.ShouldNotBeNull();
            _cacheServiceMock.Verify(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()), Times.Once);
            _orderRepositoryMock.Verify(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()), Times.Never);
            _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PagedResult<OrderDto>>(), It.IsAny<TimeSpan?>()), Times.Never);
        }

        /// <summary>
        /// Gets the seller orders returns orders when orders exist in repository.
        /// </summary>
        [Fact]
        public async Task GetSellerOrders_ReturnsOrders_WhenOrdersExistInRepository()
        {
            // Arrange
            int userId = 1;
            var role = UserRole.Seller;
            var parameters = CreateQueryParameters(userId);
            var pagedOrderEntity = CreatePagedResultOrderEntity(userId);
            var pagedOrderDto = CreatePagedResultOrderDto(userId);

            _cacheServiceMock.Setup(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<OrderDto>?)null);

            _orderRepositoryMock.Setup(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()))
                .ReturnsAsync(pagedOrderEntity);

            _mapperMock.Setup(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()))
                .Returns(pagedOrderDto);

            _cacheServiceMock.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<PagedResult<OrderDto>>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.GetSellerOrders(userId, role, parameters);

            // Assert
            result.ShouldNotBeNull();
            _cacheServiceMock.Verify(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()), Times.Once);
            _orderRepositoryMock.Verify(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()), Times.Once);
            _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()), Times.Once);
            _cacheServiceMock.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<PagedResult<OrderDto>>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        /// <summary>
        /// Gets the seller orders throws argument exception when pagination or dates are invalid.
        /// </summary>
        [Fact]
        public async Task GetSellerOrders_ThrowsArgumentException_WhenPaginationOrDatesAreInvalid()
        {
            // Arrange
            int userId = 1;
            var role = UserRole.Seller;
            var parameters = CreateQueryParameters(userId, startDate: DateTime.UtcNow.AddDays(1), page: 0, pageSize: 0);

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(async () => await _orderService.GetSellerOrders(userId, role, parameters));
            _cacheServiceMock.Verify(c => c.Get<PagedResult<OrderDto>>(It.IsAny<string>()), Times.Never);
            _orderRepositoryMock.Verify(o => o.GetSellerOrders(It.IsAny<OrderSellerQueryParameters>()), Times.Never);
            _mapperMock.Verify(m => m.Map<PagedResult<OrderDto>>(It.IsAny<PagedResult<OrderEntity>>()), Times.Never);
            _cacheServiceMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Never);
        }

        private OrderSellerQueryParameters CreateQueryParameters(int sellerId, OrderStatus? status = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            return new OrderSellerQueryParameters
            {
                SellerId = sellerId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                Page = page,
                PageSize = pageSize
            };
        }

        private PagedResult<OrderEntity> CreatePagedResultOrderEntity(int userId, int page = 1, int pageSize = 10)
        {
            return _fixture.Build<PagedResult<OrderEntity>>()
                .With(pr => pr.Page, page)
                .With(pr => pr.PageSize, pageSize)
                .With(pr => pr.Items, _fixture.Build<OrderEntity>()
                    .With(o => o.UserId, userId)
                    .Without(o => o.ShippingAddress)
                    .Without(o => o.OrderDetails)
                    .CreateMany(pageSize)
                )
                .Create();
        }

        private PagedResult<OrderDto> CreatePagedResultOrderDto(int userId, int page = 1, int pageSize = 10)
        {
            return _fixture.Build<PagedResult<OrderDto>>()
                .With(pr => pr.Page, page)
                .With(pr => pr.PageSize, pageSize)
                .With(pr => pr.Items, _fixture.Build<OrderDto>()
                    .With(o => o.UserId, userId)
                    .Without(o => o.ShippingAddress)
                    .Without(o => o.OrderDetails)
                    .CreateMany(pageSize)
                )
                .Create();
        }
    }
}