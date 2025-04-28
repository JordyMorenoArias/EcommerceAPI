using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Filters;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Handles user-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole(UserRole.Admin, UserRole.Seller, UserRole.Customer)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The user's ID.</param>
        /// <returns>The user data.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var user = await _userService.GetUserById(id);

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user data.</returns>
        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (email == default)
                return BadRequest("Email is required.");

            var user = await _userService.GetUserByEmail(email);
            return Ok(user);
        }

        /// <summary>
        /// Retrieves all users with the specified role.
        /// Only accessible by administrators.
        /// </summary>
        /// <param name="role">The user role to filter by.</param>
        /// <returns>A list of users with the given role.</returns>
        [AuthorizeRole(UserRole.Admin)]
        [HttpGet("by-role")]
        public async Task<IActionResult> GetUsersByRole([FromQuery] UserRole role)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != UserRole.Admin)
                return Unauthorized("You are not authorized to access this resource.");

            var users = await _userService.GetUsersByRole(role);

            return Ok(users);
        }

        /// <summary>
        /// Retrieves all users.
        /// Only accessible by administrators.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [AuthorizeRole(UserRole.Admin)]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        /// Updates the authenticated user's information.
        /// </summary>
        /// <param name="userUpdateDto">The user data to update.</param>
        /// <returns>The updated user data.</returns>
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var user = await _userService.UpdateUser(userAuthenticated.Id, userUpdateDto);

            return Ok(user);
        }

        /// <summary>
        /// Assigns a role to a user.
        /// Only accessible by administrators.
        /// </summary>
        /// <param name="userRoleDto">The user ID and the new role to assign.</param>
        /// <returns>The updated user data with the new role.</returns>
        [AuthorizeRole(UserRole.Admin)]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleDto userRoleDto)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            var user = await _userService.AssignRole(userRoleDto.Id, userRoleDto.Role);
            return Ok(user);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// Only administrators or the user themselves can delete.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A success message upon deletion.</returns>
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            var userAuthenticated = _userService.GetAuthenticatedUser(HttpContext);

            if (userAuthenticated.Role != Constants.UserRole.Admin || userAuthenticated.Id != id)
                return Unauthorized("You are not authorized to access this resource.");

            await _userService.DeleteUser(id);

            return Ok("User deleted successfully.");
        }
    }
}