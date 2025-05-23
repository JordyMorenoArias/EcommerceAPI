﻿using AutoMapper;
using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.User.Interfaces;

namespace EcommerceAPI.Services.User
{
    /// <summary>
    /// Provides operations related to user management, including retrieval,
    /// modification, deletion, and role assignment.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves the authenticated user's details from the HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context containing the user's claims.</param>
        /// <returns>The authenticated user's ID, email, and role.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when required claims are missing or invalid.
        /// </exception>
        public UserAuthenticatedDto GetAuthenticatedUser(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.FindFirst("Id")?.Value;
            var userEmailClaim = httpContext.User.FindFirst("Email")?.Value;
            var userRoleClaim = httpContext.User.FindFirst("Role")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userEmailClaim) || string.IsNullOrEmpty(userRoleClaim))
                throw new UnauthorizedAccessException("Invalid token or unauthorized access.");

            return new UserAuthenticatedDto
            {
                Id = int.Parse(userIdClaim),
                Email = userEmailClaim,
                Role = Enum.Parse<UserRole>(userRoleClaim)
            };
        }

        /// <summary>
        /// Retrieves a user by their unique ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A <see cref="UserDto"/> representing the user.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A list of <see cref="UserDto"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no users are found.</exception>
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();

            if (users == null || !users.Any())
                throw new KeyNotFoundException("No users found.");

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <summary>
        /// Updates the data of an existing user.
        /// </summary>
        /// <param name="userDto">The updated user data.</param>
        /// <returns>The updated <see cref="UserDto"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        /// <exception cref="Exception">Thrown when the update operation fails.</exception>
        public async Task<UserDto> UpdateUser(int userId, UserUpdateDto userUpdateDto)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            _mapper.Map(userUpdateDto, user);

            var userUpdated = await _userRepository.UpdateUser(user);

            if (userUpdated is null)
                throw new Exception("Failed to update user.");

            return _mapper.Map<UserDto>(userUpdated);
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns><c>true</c> if deletion was successful; otherwise, <c>false</c>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return await _userRepository.DeleteUser(user);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>A <see cref="UserDto"/> representing the user.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        public async Task<UserDto> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Assigns a new role to a user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="role">The role to assign.</param>
        /// <returns>The updated <see cref="UserDto"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the user is not found.</exception>
        /// <exception cref="Exception">Thrown when the update operation fails.</exception>
        public async Task<UserDto> AssignRole(int id, UserRole role)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.Role = role;
            var userUpdated = await _userRepository.UpdateUser(user);

            if (userUpdated is null)
                throw new Exception("Failed to update user.");

            return _mapper.Map<UserDto>(userUpdated);
        }

        /// <summary>
        /// Retrieves users that match the specified role.
        /// </summary>
        /// <param name="role">The role to filter users by.</param>
        /// <returns>A list of <see cref="UserDto"/> matching the role.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no users with the role are found.</exception>
        public async Task<IEnumerable<UserDto>> GetUsersByRole(UserRole role)
        {
            var users = await _userRepository.GetUsersByRole(role);

            if (users == null || !users.Any())
                throw new KeyNotFoundException("No users found.");

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}