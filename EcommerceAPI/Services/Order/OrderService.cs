using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Address.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.OrderItem.Interfaces;

namespace EcommerceAPI.Services.Order
{
    /// <summary>
    /// Service for managing orders in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Order.Interfaces.IOrderService" />
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="orderRepository">The order repository.</param>
        /// <param name="addressService">The address service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="cacheService">The cache service.</param>
        public OrderService(IOrderRepository orderRepository, IAddressRepository addressRepository, ICacheService cacheService, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="role">The role.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to access this order.</exception>
        public async Task<OrderDto?> GetOrderById(int userid, UserRole role, int id)
        {
            var cacheKey = $"order_{id}";
            var cachedOrder = await _cacheService.Get<OrderDto>(cacheKey);

            if (cachedOrder is not null)
                return cachedOrder;

            var order = await _orderRepository.GetOrderById(id);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            if (role == UserRole.Customer && order.UserId != userid)
                throw new UnauthorizedAccessException("You do not have permission to access this order.");

            if (role == UserRole.Seller && order.ShippingAddress.UserId != userid)
                throw new UnauthorizedAccessException("You do not have permission to access this order.");

            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders matching the provided filters as <see cref="PagedResult{OrderDto}"/>.</returns>
        /// <exception cref="System.UnauthorizedAccessException">You do not have permission to access orders for this user.</exception>
        public async Task<PagedResult<OrderDto>> GetOrders(int userId, UserRole role, OrderQueryParameters parameters)
        {
            ValidatePaginationAndDates(
                parameters.Page,
                parameters.PageSize,
                parameters.StartDate,
                parameters.EndDate
            );

            if (parameters.UserId.HasValue && parameters.UserId != userId && role != UserRole.Admin)
                throw new UnauthorizedAccessException("You do not have permission to access orders for this user.");

            var cacheKey = BuildCacheKey(
                prefix: "orders",
                startDate: parameters.StartDate,
                endDate: parameters.EndDate,
                userId: parameters.UserId,
                sellerId: null,
                status: parameters.Status,
                page: parameters.Page,
                pageSize: parameters.PageSize
            );

            var cachedOrders = await _cacheService.Get<PagedResult<OrderDto>>(cacheKey);

            if (cachedOrders is not null)
                return cachedOrders;

            var orders = await _orderRepository.GetOrders(parameters);
            var orderDtos = _mapper.Map<PagedResult<OrderDto>>(orders);

            await _cacheService.Set(cacheKey, orderDtos, TimeSpan.FromMinutes(5));
            return orderDtos;
        }

        /// <summary>
        /// Gets the seller orders.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paginated list of orders for a specific seller as <see cref="PagedResult{OrderDto}"/>.</returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// You do not have permission to access orders for this seller.
        /// or
        /// You do not have permission to access seller orders.
        /// </exception>
        public async Task<PagedResult<OrderDto>> GetSellerOrders(int userId, UserRole role, OrderSellerQueryParameters parameters)
        {
            ValidatePaginationAndDates(
                parameters.Page,
                parameters.PageSize,
                parameters.StartDate,
                parameters.EndDate
            );

            if (parameters.SellerId != userId && role != UserRole.Admin)
                throw new UnauthorizedAccessException("You do not have permission to access orders for this seller.");

            if (role != UserRole.Seller && role != UserRole.Admin)
                throw new UnauthorizedAccessException("You do not have permission to access seller orders.");

            var cacheKey = BuildCacheKey(
                prefix: "orders_seller",
                startDate: parameters.StartDate,
                endDate: parameters.EndDate,
                userId: null,
                sellerId: parameters.SellerId,
                status: parameters.Status,
                page: parameters.Page,
                pageSize: parameters.PageSize
            );

            var cachedOrders = await _cacheService.Get<PagedResult<OrderDto>>(cacheKey);

            if (cachedOrders is not null)
                return cachedOrders;

            var orders = await _orderRepository.GetSellerOrders(parameters);
            var orderDtos = _mapper.Map<PagedResult<OrderDto>>(orders);

            await _cacheService.Set(cacheKey, orderDtos, TimeSpan.FromMinutes(5));
            return orderDtos;
        }

        /// <summary>
        /// Validates the pagination and dates.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <exception cref="System.ArgumentException">
        /// PageSize cannot exceed {MaxPageSize}.
        /// or
        /// Page and PageSize must be greater than 0.
        /// or
        /// Start date must be less than or equal to end date.
        /// or
        /// Dates cannot be in the future.
        /// </exception>
        private void ValidatePaginationAndDates(int page, int pageSize, DateTime? startDate, DateTime? endDate)
        {
            const int MaxPageSize = 100;

            if (pageSize > MaxPageSize)
                throw new ArgumentException($"PageSize cannot exceed {MaxPageSize}.");

            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ArgumentException("Start date must be less than or equal to end date.");

            if ((startDate.HasValue && startDate > DateTime.UtcNow) ||
                (endDate.HasValue && endDate > DateTime.UtcNow))
                throw new ArgumentException("Dates cannot be in the future.");
        }

        /// <summary>
        /// Builds the cache key.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sellerId">The seller identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        private string BuildCacheKey(string prefix, DateTime? startDate, DateTime? endDate, int? userId, int? sellerId, OrderStatus? status, int page, int pageSize)
        {
            var dateRangePart = startDate.HasValue && endDate.HasValue
                ? $"{startDate:yyyyMMdd}_{endDate:yyyyMMdd}"
                : "all_dates";

            var userIdPart = userId.HasValue ? userId.Value.ToString() : "all_users";
            var sellerIdPart = sellerId.HasValue ? sellerId.Value.ToString() : "all_sellers";
            var statusPart = status.HasValue ? status.Value.ToString() : "all_statuses";

            return $"{prefix}_{dateRangePart}_{userIdPart}_{sellerIdPart}_{statusPart}_Page_{page}_PageSize_{pageSize}";
        }

        /// <summary>
        /// Gets the order with details.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto?> GetOrderWithDetails(int userId, UserRole role, int orderId)
        {
            var cacheKey = $"order_details_{orderId}";
            var cachedOrder = await _cacheService.Get<OrderDto>(cacheKey);

            if (cachedOrder is not null)
                return cachedOrder;

            var order = await _orderRepository.GetOrderWithDetails(orderId);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Adds the order.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The newly created draft order as <see cref="OrderDto"/>.</returns>
        /// <exception cref="System.InvalidOperationException">User does not have a default address</exception>
        public async Task<OrderDto> AddOrder(int userId)
        {
            var defaultAddress = await _addressRepository.GetDefaultAddressForUserAsync(userId);

            if (defaultAddress is null)
                throw new InvalidOperationException("User does not have a default address");

            var order = new OrderEntity
            {
                UserId = userId,
                ShippingAddressId = defaultAddress.Id,
                TotalAmount = 0.0m,
                Status = OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            var createdOrder = await _orderRepository.AddOrder(order);

            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            await SaveOrderToCache(orderDto);
            return orderDto;
        }

        /// <summary>
        /// Updates the address order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns>The updated order with the new shipping address as <see cref="OrderDto"/>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto> UpdateAddressOrder(int orderId, int addressId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            var address = await _addressRepository.GetAddressById(addressId);

            if (address is null)
                throw new KeyNotFoundException("Address not found");

            order.ShippingAddressId = addressId;
            var orderUpdate = await _orderRepository.UpdateOrder(order);

            var orderDto = _mapper.Map<OrderDto>(orderUpdate);
            await InvalidateOrderCache(orderDto.Id);
            await SaveOrderToCache(orderDto);
            return orderDto;
        }

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>The updated order with the new status as <see cref="OrderDto"/>.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            order.Status = newStatus;
            var orderUpdate = await _orderRepository.UpdateOrder(order);

            var orderDto = _mapper.Map<OrderDto>(order);
            await InvalidateOrderCache(orderDto.Id);
            await SaveOrderToCache(orderDto);
            return orderDto;
        }

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns><c>true</c> if the order was successfully deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteOrder(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            await InvalidateOrderCache(order.Id);
            return await _orderRepository.DeleteOrder(order);
        }

        /// <summary>
        /// Saves the order to cache.
        /// </summary>
        /// <param name="orderDto">The order dto.</param>
        /// <exception cref="System.ArgumentNullException">orderDto</exception>
        private async Task SaveOrderToCache(OrderDto orderDto)
        {
            var cacheKey = $"order_{orderDto.Id}";
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Invalidates the order cache.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        private async Task InvalidateOrderCache(int orderId)
        {
            // Eliminar la cache específica de la orden
            await _cacheService.Remove($"order_{orderId}");
            await _cacheService.Remove($"order_details_{orderId}");

            // Invalida total de ventas, porque una actualización puede afectarlo
            await _cacheService.Remove("total_sales");
        }
    }
}