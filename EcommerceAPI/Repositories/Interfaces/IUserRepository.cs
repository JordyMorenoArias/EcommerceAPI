using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUser(User user);
        Task<bool> ConfirmUser(string token);
        Task<bool> DeleteUser(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
        Task<bool> UpdateUser(User user);
    }
}