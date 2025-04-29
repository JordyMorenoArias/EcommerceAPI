using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.User.Interfaces
{
    /// <summary>
    /// Interface for User Service that manages user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Assigns a specific role to a user.
        /// </summary>
        /// <param name="id">The identifier of the user.</param>
        /// <param name="role">The role to assign to the user.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="UserDto"/> containing the updated user details.</returns>
        Task<UserDto> AssignRole(int id, UserRole role);

        /// <summary>
        /// Deletes a user by their identifier.
        /// </summary>
        /// <param name="id">The identifier of the user to delete.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean value indicating whether the user was successfully deleted.</returns>
        Task<bool> DeleteUser(int id);

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, with an <see cref="IEnumerable{UserDto}"/> containing the list of all users.</returns>
        Task<IEnumerable<UserDto>> GetAllUsers();

        /// <summary>
        /// Retrieves the authenticated user from the HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context containing the current user's information.</param>
        /// <returns>An instance of <see cref="UserAuthenticatedDto"/> representing the authenticated user's details.</returns>
        UserAuthenticatedDto GetAuthenticatedUser(HttpContext httpContext);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="UserDto"/> containing the user details.</returns>
        Task<UserDto> GetUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their identifier.
        /// </summary>
        /// <param name="id">The identifier of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="UserDto"/> containing the user details.</returns>
        Task<UserDto> GetUserById(int id);

        /// <summary>
        /// Retrieves all users with a specific role.
        /// </summary>
        /// <param name="role">The role to filter users by.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="IEnumerable{UserDto}"/> containing the list of users with the specified role.</returns>
        Task<IEnumerable<UserDto>> GetUsersByRole(UserRole role);

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="userId">The identifier of the user to update.</param>
        /// <param name="userUpdateDto">The data transfer object containing the updated user information.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="UserDto"/> containing the updated user details.</returns>
        Task<UserDto> UpdateUser(int userId, UserUpdateDto userUpdateDto);
    }
}