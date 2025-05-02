using EcommerceAPI.Constants;
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
        /// Confirms the user with the provided token.
        /// </summary>
        /// <param name="token">The confirmation token for the user.</param>
        /// <returns>True if the user is confirmed, otherwise false.</returns>
        Task<bool> ConfirmUser(string token);

        /// <summary>
        /// Deletes a user from the repository.
        /// </summary>
        /// <param name="user">The user entity to be deleted.</param>
        /// <returns>True if the user was deleted, otherwise false.</returns>
        Task<bool> DeleteUser(UserEntity user);

        /// <summary>
        /// Retrieves all users from the repository.
        /// </summary>
        /// <returns>An enumerable collection of all users.</returns>
        Task<IEnumerable<UserEntity>> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user entity if found, otherwise null.</returns>
        Task<UserEntity?> GetUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user entity if found, otherwise null.</returns>
        Task<UserEntity?> GetUserById(int id);

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
        Task<UserEntity?> UpdateUser(UserEntity user);
    }
}