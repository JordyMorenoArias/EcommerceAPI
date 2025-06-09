namespace EcommerceAPI.Services.Security.Interfaces
{
    /// <summary>
    /// Interface for token generation service that handles token creation.
    /// </summary>
    public interface ITokenGenerator
    {
        /// <summary>
        /// Generates a 6-digit token.
        /// </summary>
        /// <returns>A 6-digit token.</returns>
        string Generate6DigitToken();

        /// <summary>
        /// Generates a general token as a string.
        /// </summary>
        /// <returns>A string containing the generated token.</returns>
        string GenerateToken();
    }
}