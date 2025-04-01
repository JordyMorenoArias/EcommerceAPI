using EcommerceAPI.Models.DTOs.User;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IUserService
    {
        UserAuthenticatedDto GetAuthenticatedUser(HttpContext httpContext);
    }
}