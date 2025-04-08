using EcommerceAPI.Constants;
using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AssignRole(int id, UserRole role);
        Task<bool> DeleteUser(int id);
        Task<IEnumerable<UserDto>> GetAllUsers();
        UserAuthenticatedDto GetAuthenticatedUser(HttpContext httpContext);
        Task<UserDto> GetUserByEmail(string email);
        Task<UserDto> GetUserById(int id);
        Task<IEnumerable<UserDto>> GetUsersByRole(UserRole role);
        Task<UserDto> UpdateUser(UserDto userDto);
    }
}