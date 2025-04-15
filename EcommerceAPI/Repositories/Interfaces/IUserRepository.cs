using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity> AddUser(UserEntity user);
        Task<bool> ConfirmUser(string token);
        Task<bool> DeleteUser(UserEntity user);
        Task<IEnumerable<UserEntity>> GetAllUsers();
        Task<UserEntity?> GetUserByEmail(string email);
        Task<UserEntity?> GetUserById(int id);
        Task<IEnumerable<UserEntity>> GetUsersByRole(UserRole role);
        Task<UserEntity?> UpdateUser(UserEntity user);
    }
}