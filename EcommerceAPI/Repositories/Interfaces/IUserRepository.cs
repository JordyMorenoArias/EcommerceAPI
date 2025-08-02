using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.Generic;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    /// <summary>
    /// Interface for user repository operations.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a new user to the repository.
        /// </summary>
        /// <param name="user">The user entity to be added.</param>
        /// <returns>The added user entity.</returns>
        Task<UserEntity> AddUser(UserEntity user);

        /// <summary>
        /// Deletes a user from the repository.
        /// </summary>
        /// <param name="user">The user entity to be deleted.</param>
        /// <returns>True if the user was deleted, otherwise false.</returns>
        Task<bool> DeleteUser(UserEntity user);

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user entity if found, otherwise null.</returns>
        Task<UserEntity?> GetUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the user if found; otherwise, <c>null</c>.</returns>
        Task<UserEntity?> GetUserById(int id);

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A paged result containing a list of users that match the given parameters.</returns>
        Task<PagedResult<UserEntity>> GetUsers(QueryUserParameters parameters);

        /// <summary>
        /// Retrieves users based on their role.
        /// </summary>
        /// <param name="role">The role of the users to retrieve.</param>
        /// <returns>An enumerable collection of users with the specified role.</returns>
        Task<IEnumerable<UserEntity>> GetUsersByRole(UserRole role);

        /// <summary>
        /// Updates an existing user in the repository.
        /// </summary>
        /// <param name="user">The user entity with updated information.</param>
        /// <returns>The updated user entity, or null if the update failed.</returns>
        Task<UserEntity> UpdateUser(UserEntity user);
    }
}