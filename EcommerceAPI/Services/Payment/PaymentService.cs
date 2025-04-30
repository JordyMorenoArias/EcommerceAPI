using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Infrastructure.Interfaces;
using EcommerceAPI.Services.Order.Interfaces;
using EcommerceAPI.Services.Payment.Interfaces;

namespace EcommerceAPI.Services.Payment
{
    /// <summary>
    /// Service for processing payments.
    /// </summary>
    /// <seealso cref="EcommerceAPI.Services.Payment.Interfaces.IPaymentService" />
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderService _orderService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="paymentRepository">The payment repository.</param>
        /// <param name="orderService">The order service.</param>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="mapper">The mapper.</param>
        public PaymentService(IPaymentRepository paymentRepository, IOrderService orderService, ICacheService cacheService, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets the payment by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Payment not found</exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not authorized to view this payment.</exception>
        public async Task<PaymentEntity> GetPaymentById(int userId, UserRole role, int id)
        {
            var cacheKey = $"Payment_{id}";
            var cachedPayment = await _cacheService.Get<PaymentEntity>(cacheKey);

            if (cachedPayment is not null && IsAuthorized(cachedPayment, userId, role))
                return cachedPayment;

            var payment = await _paymentRepository.GetPaymentById(id);

            if (payment is null)
                throw new Exception("Payment not found");

            if (!IsAuthorized(payment, userId, role))
                throw new UnauthorizedAccessException("You are not authorized to view this payment.");

            await _cacheService.Set(cacheKey, payment, TimeSpan.FromMinutes(10));
            return payment;
        }

        private bool IsAuthorized(PaymentEntity payment, int userId, UserRole role) => payment.Order.UserId == userId || role == UserRole.Admin;


        /// <summary>Gets the payments.</summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// A <see cref="PagedResult{PaymentProcessDto}"/> containing the paginated list of payments matching the query parameters.
        /// </returns>
        /// <exception cref="System.ArgumentException">Page and PageSize must be greater than 0.
        /// or
        /// StartDate must be less than or equal to EndDate.</exception>
        /// <exception cref="System.UnauthorizedAccessException">You are not authorized to view these payments.</exception>
        public async Task<PagedResult<PaymentProcessDto>> GetPayments(int userId, UserRole role, PaymentQueryParameters parameters)
        {
            if (parameters.Page <= 0 || parameters.PageSize <= 0)
                throw new ArgumentException("Page and PageSize must be greater than 0.");

            if (parameters.StartDate.HasValue && parameters.EndDate.HasValue && parameters.StartDate > parameters.EndDate)
                throw new ArgumentException("StartDate must be less than or equal to EndDate.");

            if (role != UserRole.Admin && parameters.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to view these payments.");

            var dateRangePart = parameters.StartDate.HasValue && parameters.EndDate.HasValue
                ? $"{parameters.StartDate:yyyyMMdd}_{parameters.EndDate:yyyyMMdd}"
                : "all_dates";

            var statusPart = parameters.Status.HasValue 
                ? parameters.Status.ToString() 
                : "all_statuses";

            var userIdPart = parameters.UserId.HasValue 
                ? parameters.UserId.ToString() 
                : "all_users";

            var cacheKey = $"Payments_{userIdPart}_{statusPart}_{dateRangePart}_Page_{parameters.Page}_PageSize_{parameters.PageSize}";
            var cachedPayments = await _cacheService.Get<PagedResult<PaymentProcessDto>>(cacheKey);

            if (cachedPayments is not null)
                return cachedPayments;

            var payments = await _paymentRepository.GetPayments(parameters);
            var paymentDtos = _mapper.Map<PagedResult<PaymentProcessDto>>(payments);

            await _cacheService.Set(cacheKey, paymentDtos, TimeSpan.FromMinutes(3));
            return paymentDtos;
        }

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="paymentDto">The payment dto.</param>
        /// <returns>
        /// A <see cref="PaymentResultDto"/> object containing the result of the payment transaction.
        /// </returns>
        /// <exception cref="System.Exception">
        /// Order not found
        /// or
        /// Order already paid
        /// or
        /// Order is canceled
        /// or
        /// User not authorized to pay for this order
        /// </exception>
        public async Task<PaymentResultDto> ProcessPayment(int userId, int orderId, PaymentProcessDto paymentDto)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order is null)
                throw new Exception("Order not found");

            if (order.Status == OrderStatus.Paid)
                throw new Exception("Order already paid");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order is canceled");

            if (order.UserId != userId)
                throw new Exception("User not authorized to pay for this order");

            // Simulación de pago exitoso
            var transaction = SimulatePayment(order.TotalAmount, paymentDto);

            // var transaction = await _paymentGatewayService.ProcessPaymentAsync(paymentDto, order.TotalAmount);

            var payment = new PaymentEntity
            {
                OrderId = orderId,
                Amount = transaction.Amount,
                Method = paymentDto.Method,
                Status = transaction.Status,
                LastFourDigits = transaction.LastFourDigits,
                TransactionId = transaction.TransactionId
            };

            var result = await _paymentRepository.AddPayment(payment);
            await _orderService.UpdateOrderStatus(order.Id, OrderStatus.Paid);

            return transaction;
        }

        private PaymentResultDto SimulatePayment(decimal amount, PaymentProcessDto dto)
        {
            return new PaymentResultDto
            {
                PaymentId = 0,
                TransactionId = Guid.NewGuid().ToString(),
                Status = PaymentStatus.Paid,
                Message = "Simulated payment successful",
                Amount = amount,
                Currency = "USD",
                PaymentMethod = dto.Method,
                CardProvider = CardProvider.Visa,
                LastFourDigits = dto.CardNumber[^4..],
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
