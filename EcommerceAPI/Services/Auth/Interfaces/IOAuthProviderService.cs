using EcommerceAPI.Models.DTOs.Auth;

namespace EcommerceAPI.Services.Auth.Interfaces
{
    /// <summary>
    /// Interface for OAuth provider service that handles OAuth authentication.
    /// </summary>
    public interface IOAuthProviderService
    {
        /// <summary>
        /// Retrieves the user information using the provided OAuth authorization code.
        /// </summary>
        /// <param name="code">The OAuth authorization code received from the OAuth provider.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AuthResponseDto"/> containing the user information.</returns>
        Task<AuthResponseDto> GetUserInfoAsync(string code);
    }
}