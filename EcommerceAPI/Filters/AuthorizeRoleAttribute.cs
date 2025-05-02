using EcommerceAPI.Constants;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EcommerceAPI.Filters
{
    /// <summary>
    /// Authorization filter that checks if the user has the required role.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _roles;

        public AuthorizeRoleAttribute(params UserRole[] roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
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
