namespace InternPulse4.Core.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken, string userId);
    }
}
