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
    [AuthorizeRole(UserRole.Customer)]
    public class AddressController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        public AddressController(IUserService userService, IAddressService addressService)
        {
            _userService = userService;
            _addressService = addressService;
        }

        /// <summary>
        /// Gets the address by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAddressById([FromQuery] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.GetAddressById(userAuthenticated.Id, id);

            return Ok(address);
        }

        /// <summary>
        /// Gets the addresses by user identifier.
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        public async Task<IActionResult> GetAddressesByUserId()
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var addresses = await _addressService.GetAddressesByUserId(userAuthenticated.Id);
            return Ok(addresses);
        }

        /// <summary>
        /// Gets the default address by user identifier.
        /// </summary>
        /// <returns></returns>
        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddressByUserId()
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var defaultAddress = await _addressService.GetDefaultAddressForUser(userAuthenticated.Id);
            return Ok(defaultAddress);
        }

        /// <summary>
        /// Adds the address.
        /// </summary>
        /// <param name="addressAdd">The address add.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressAddDto addressAdd)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.AddAddress(userAuthenticated.Id, addressAdd);
            return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, address);
        }

        /// <summary>
        /// Updates the address.
        /// </summary>
        /// <param name="addressUpdate">The address update.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateDto addressUpdate)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.UpdateAddress(userAuthenticated.Id, addressUpdate);
            return Ok(address);
        }

        /// <summary>
        /// Deletes the address.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteAddress([FromQuery] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var result = await _addressService.DeleteAddress(userAuthenticated.Id, id);

            if (result)
                return NoContent();

            return NotFound("Address not found");
        }

        /// <summary>
        /// Sets the default address.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPut("default/{id}")]
        public async Task<IActionResult> SetDefaultAddress([FromRoute] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);
            var address = await _addressService.SetDefaultAddress(userAuthenticated.Id, id);
            return Ok(address);
        }
    }
}
