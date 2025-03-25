using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User?> ConfirmUser(string token);
        Task<bool> DeleteUser(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<bool> UpdateUser(User user);
    }
}