using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.Address;
using EcommerceAPI.Services.Address.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing address-related operations.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressController"/> class.
        /// </summary>
        /// <param name="userService">Service for retrieving authenticated user information.</param>
        /// <param name="addressService">Service for managing address data.</param>
        public AddressController(IUserService userService, IAddressService addressService)
        {
            _userService = userService;
            _addressService = addressService;
        }

        /// <summary>
        /// Retrieves an address by its unique identifier, based on the role of the authenticated user.
        /// </summary>
        /// <param name="id">The unique identifier of the address.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the address if found, otherwise a not found result.
        /// </returns>
        [HttpGet("{id}")]
        [AuthorizeRole(UserRole.Customer, UserRole.Admin, UserRole.Seller)]
        public async Task<IActionResult> GetAddressById([FromRoute] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.GetAddressById(userAuthenticated.Id, userAuthenticated.Role, id);
            return Ok(address);
        }

        /// <summary>
        /// Retrieves all addresses associated with the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of the user's addresses.
        /// </returns>
        [HttpGet]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> GetAddressesByUserId()
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var addresses = await _addressService.GetAddressesByUserId(userAuthenticated.Id);
            return Ok(addresses);
        }

        /// <summary>
        /// Retrieves the default address for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the default address.
        /// </returns>
        [HttpGet("default")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> GetDefaultAddressByUserId()
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var defaultAddress = await _addressService.GetDefaultAddressForUser(userAuthenticated.Id);
            return Ok(defaultAddress);
        }

        /// <summary>
        /// Adds a new address for the authenticated user.
        /// </summary>
        /// <param name="addressAdd">The address data to be added.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> with the created address and location header.
        /// </returns>
        [HttpPost]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> AddAddress([FromBody] AddressAddDto addressAdd)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.AddAddress(userAuthenticated.Id, addressAdd);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        /// <summary>
        /// Updates an existing address for the authenticated user.
        /// </summary>
        /// <param name="addressUpdate">The updated address data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated address.
        /// </returns>
        [HttpPut]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateDto addressUpdate)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.UpdateAddress(userAuthenticated.Id, addressUpdate);
            return Ok(address);
        }

        /// <summary>
        /// Deletes an address by its identifier for the authenticated user.
        /// </summary>
        /// <param name="id">The unique identifier of the address to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> on success or <see cref="NotFoundResult"/> if not found.
        /// </returns>
        [HttpDelete]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> DeleteAddress([FromQuery] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _addressService.DeleteAddress(userAuthenticated.Id, id);

            if (result)
                return NoContent();

            return NotFound("Address not found");
        }

        /// <summary>
        /// Sets a specific address as the default for the authenticated user.
        /// </summary>
        /// <param name="id">The unique identifier of the address to set as default.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated default address.
        /// </returns>
        [HttpPut("default/{id}")]
        [AuthorizeRole(UserRole.Customer)]
        public async Task<IActionResult> SetDefaultAddress([FromRoute] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.SetDefaultAddress(userAuthenticated.Id, id);
            return Ok(address);
        }
    }
}