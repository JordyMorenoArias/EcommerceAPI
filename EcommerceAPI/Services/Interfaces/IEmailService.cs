namespace EcommerceAPI.Services.Interfaces
{
    public interface IEmailService
    {
        bool SendForgotPassword(string email);
        bool SendVerificationEmail(string email, string token);
    }
}