using AutoMapper;
using EcommerceAPI.Models.DTOs.OrderDetail;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.OrderItem.Interfaces;

namespace EcommerceAPI.Services.OrderItem
{
    /// <summary>
    /// Service for managing order details in the e-commerce system.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.OrderItem.Interfaces.IOrderDetailService" />
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailService"/> class.
        /// </summary>
        /// <param name="orderDetailRepository">The order detail repository.</param>
        /// <param name="mapper">The object mapper.</param>
        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the order detail by ID.
        /// </summary>
        /// <param name="id">The identifier of the order detail.</param>
        /// <returns>The order detail DTO.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when no order detail with the specified ID is found.</exception>
        public async Task<OrderDetailDto> GetOrderDetailById(int id)
        {
            var orderDto = await _orderDetailRepository.GetOrderDetailById(id);

            if (orderDto is null)
                throw new KeyNotFoundException($"Order detail with ID {id} not found.");

            return _mapper.Map<OrderDetailDto>(orderDto);
        }

        /// <summary>
        /// Adds a collection of order details to an order.
        /// </summary>
        /// <param name="orderId">The identifier of the order to which the details will be added.</param>
        /// <param name="orderDetails">The collection of order detail entities.</param>
        /// <returns>A result DTO indicating the result of the operation.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the order details collection is null or empty.</exception>
        public async Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var result = await _orderDetailRepository.AddOrderDetails(orderId, orderDetails);
            return result;
        }
    }
}