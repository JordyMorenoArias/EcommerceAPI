using EcommerceAPI.Constants;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EcommerceAPI.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _roles;

        public AuthorizeRoleAttribute(params UserRole[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userService = context.HttpContext.RequestServices.GetService(typeof(IUserService)) as IUserService;
            var user = userService?.GetAuthenticatedUser(context.HttpContext);

            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult("Authentication is required.");
                return;
            }

            if (!_roles.Contains(user.Role))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
