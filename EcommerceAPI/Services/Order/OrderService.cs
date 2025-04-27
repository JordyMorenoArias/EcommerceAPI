using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.Entities;
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
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public OrderService(IOrderRepository orderRepository, IAddressService addressService, IMapper mapper, ICacheService cacheService)
        {
            _orderRepository = orderRepository;
            _addressService = addressService;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Gets the orders by date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Start date must be less than or equal to end date</exception>
        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be less than or equal to end date");

            var cacheKey = $"orders_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
            var cachedOrders = await _cacheService.Get<IEnumerable<OrderDto>>(cacheKey);

            if (cachedOrders is not null)
                return cachedOrders;

            var orders = _orderRepository.GetOrdersByDateRange(startDate, endDate);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            await _cacheService.Set(cacheKey, orderDtos, TimeSpan.FromMinutes(10));
            return orderDtos;
        }

        /// <summary>
        /// Gets the order by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto?> GetOrderById(int id)
        {
            var cacheKey = $"order_{id}";
            var cachedOrder = await _cacheService.Get<OrderDto>(cacheKey);

            if (cachedOrder is not null)
                return cachedOrder;

            var order = await _orderRepository.GetOrderById(id);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Gets the orders by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(int userId)
        {
            var cacheKey = $"user_orders_{nameof(userId)}";
            var cachedOrders = await _cacheService.Get<IEnumerable<OrderDto>>(cacheKey);

            if (cachedOrders is not null)
                return cachedOrders;

            var orders = await _orderRepository.GetOrdersByUserId(userId);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            await _cacheService.Set(cacheKey, orderDtos, TimeSpan.FromMinutes(5));
            return orderDtos;
        }

        /// <summary>
        /// Gets the orders by status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDto>> GetOrdersByStatus(OrderStatus status)
        {
            var cacheKey = $"orders_status_{nameof(status)}";
            var cachedOrders = await _cacheService.Get<IEnumerable<OrderDto>>(cacheKey);

            if (cachedOrders is not null)
                return cachedOrders;

            var orders = await _orderRepository.GetOrdersByStatus(status);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            await _cacheService.Set(cacheKey, orderDtos, TimeSpan.FromMinutes(3));
            return orderDtos;
        }

        /// <summary>
        /// Gets the order with details.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto?> GetOrderWithDetails(int orderId)
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
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">User does not have a default address</exception>
        public async Task<OrderDto> AddOrder(int userId)
        {
            var defaultAddress = await _addressService.GetDefaultAddressForUser(userId);

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

            var cacheKey = $"order_{createdOrder.Id}";
            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto; ;
        }

        /// <summary>
        /// Updates the amount order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Amount cannot be negative</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto> UpdateAmountOrder(int orderId, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");

            var order = await _orderRepository.UpdateAmountOrder(orderId, amount);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            await InvalidateOrderCache(orderId);

            var cacheKey = $"order_{orderId}";
            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Updates the address order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="addressId">The address identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto> UpdateAddressOrder(int orderId, int addressId)
        {

            var order = await _orderRepository.UpdateAddressOrder(orderId, addressId);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            await InvalidateOrderCache(orderId);

            var cacheKey = $"order_{orderId}";
            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Updates the order status.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order not found</exception>
        public async Task<OrderDto> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {

            var order = await _orderRepository.UpdateOrderStatus(orderId, newStatus);

            if (order is null)
                throw new KeyNotFoundException("Order not found");

            await InvalidateOrderCache(orderId, null, newStatus);

            var cacheKey = $"order_{orderId}";
            var orderDto = _mapper.Map<OrderDto>(order);
            await _cacheService.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));
            return orderDto;
        }

        /// <summary>
        /// Deletes the order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeleteOrder(int orderId)
        {
            await InvalidateOrderCache(orderId);
            return await _orderRepository.DeleteOrder(orderId);
        }

        /// <summary>
        /// Gets the total sales.
        /// </summary>
        /// <returns></returns>
        public async Task<decimal> GetTotalSales()
        {
            var cacheKey = "total_sales";
            decimal? cachedTotalSales = await _cacheService.Get<decimal?>(cacheKey);

            if (cachedTotalSales.HasValue)
                return cachedTotalSales.Value;

            var totalSales = await _orderRepository.GetTotalSales();
            await _cacheService.Set(cacheKey, totalSales, TimeSpan.FromMinutes(2));
            return totalSales;
        }

        /// <summary>
        /// Invalidates the order cache.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="status">The status.</param>
        private async Task InvalidateOrderCache(int orderId, int? userId = null, OrderStatus? status = null)
        {
            // Eliminar la cache específica de la orden
            await _cacheService.Remove($"order_{orderId}");
            await _cacheService.Remove($"order_details_{orderId}");

            // Opcionalmente eliminar la cache de las órdenes del usuario
            if (userId.HasValue)
                await _cacheService.Remove($"user_orders_{userId.Value}");

            // Opcionalmente eliminar la cache de las órdenes por estado
            if (status.HasValue)
                await _cacheService.Remove($"orders_status_{nameof(status.Value)}");

            // Invalida total de ventas, porque una actualización puede afectarlo
            await _cacheService.Remove("total_sales");
        }
    }
}