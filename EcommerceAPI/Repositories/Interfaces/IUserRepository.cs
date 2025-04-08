using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> AddUser(User user);
        Task<bool> ConfirmUser(string token);
        Task<bool> DeleteUser(User user);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<IEnumerable<User>> GetUsersByRole(UserRole role);
        Task<UserDto?> UpdateUser(User user);
    }
}