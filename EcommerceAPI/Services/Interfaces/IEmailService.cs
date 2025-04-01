namespace EcommerceAPI.Services.Interfaces
{
    public interface IEmailService
    {
        bool SendForgotPassword(string email, int code);
        bool SendVerificationEmail(string email, string token);
    }
}