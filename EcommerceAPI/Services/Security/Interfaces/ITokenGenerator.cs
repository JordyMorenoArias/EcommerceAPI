namespace EcommerceAPI.Services.Security.Interfaces
{
    public interface ITokenGenerator
    {
        int Generate6DigitToken();
        string GenerateToken();
    }
}