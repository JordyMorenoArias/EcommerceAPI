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

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the order detail by i.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Order detail with ID {id} not found.</exception>
        public async Task<OrderDetailDto> GetOrderDetailByI(int id)
        {
            var orderDto = await _orderDetailRepository.GetOrderDetailById(id);

            if (orderDto is null)
                throw new KeyNotFoundException($"Order detail with ID {id} not found.");

            return _mapper.Map<OrderDetailDto>(orderDto);
        }

        /// <summary>
        /// Adds the order details.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderDetails">The order details.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Order details cannot be null or empty. - orderDetails</exception>
        public async Task<AddOrderDetailResultDto> AddOrderDetails(int orderId, IEnumerable<OrderDetailEntity> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order details cannot be null or empty.", nameof(orderDetails));

            var result = await _orderDetailRepository.AddOrderDetails(orderId, orderDetails);
            return result;
        }
    }
}