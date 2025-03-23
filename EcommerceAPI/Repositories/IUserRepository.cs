using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User?> ConfirmUser(string token);
        Task<User?> DeleteUser(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<User?> UpdateUser(User user);
    }
}