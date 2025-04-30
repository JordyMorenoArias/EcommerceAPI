using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Payment;
using EcommerceAPI.Services.Payment.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller for managing payments in the e-commerce system.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Customer, UserRole.Admin)]
    public class PaymentController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="paymentService">The payment service.</param>
        public PaymentController(IUserService userService, IPaymentService paymentService)
        {
            _userService = userService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Gets the payment by its identifier for the authenticated user.
        /// </summary>
        /// <param name="id">The identifier of the payment.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the payment data if authorized and found.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var payment = await _paymentService.GetPaymentById(userAuthenticated.Id, userAuthenticated.Role, id);
            return Ok(payment);
        }

        /// <summary>
        /// Gets a paginated list of payments based on query parameters for the authenticated user.
        /// </summary>
        /// <param name="parameters">The parameters used for filtering and pagination.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the paginated list of payments.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetPayments([FromQuery] PaymentQueryParameters parameters)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var payments = await _paymentService.GetPayments(userAuthenticated.Id, userAuthenticated.Role, parameters);
            return Ok(payments);
        }

        /// <summary>
        /// Processes a payment for the authenticated customer.
        /// </summary>
        /// <param name="request">The payment request, including order ID and payment details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the result of the payment transaction.
        /// </returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var paymentResult = await _paymentService.ProcessPayment(userAuthenticated.Id, request.OrderId, request.PaymentDto);
            return Ok(paymentResult);
        }
    }
}
