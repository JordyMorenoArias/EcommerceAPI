using EcommerceAPI.Constants;
using EcommerceAPI.Data;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repositories
{
    /// <summary>
    /// Repository for managing user data in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly EcommerceContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context for e-commerce operations.</param>
        public UserRepository(EcommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<UserEntity?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<UserEntity?> GetUserByEmail(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user entity to add.</param>
        /// <returns>The added user.</returns>
        public async Task<UserEntity> AddUser(UserEntity user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user entity with updated information.</param>
        /// <returns>The updated user if successful; otherwise, null.</returns>
        public async Task<UserEntity?> UpdateUser(UserEntity user)
        {
            var existingUser = await GetUserById(user.Id);
            if (existingUser is null)
                return null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>The deleted user if found; otherwise, null.</returns>
        public async Task<bool> DeleteUser(UserEntity user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Confirms a user's email using a confirmation token.
        /// </summary>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>The confirmed user if successful; otherwise, null.</returns>
        public async Task<bool> ConfirmUser(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmedToken == token);

            if (user != null)
            {
                user.IsEmailConfirmed = true;
                user.EmailConfirmedToken = string.Empty;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Gets the users by role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserEntity>> GetUsersByRole(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
