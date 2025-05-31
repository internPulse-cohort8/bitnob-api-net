using System.Net.Mail;
using System.Net;
using InternPulse4.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace InternPulse4.Core.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Validate recipient email
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(to));

            try
            {
                // Try parsing to validate email format
                var recipient = new MailAddress(to);

                var smtp = new SmtpClient(_smtpSettings.Host)
                {
                    Port = _smtpSettings.Port,
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl,
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.From),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(recipient);

                await smtp.SendMailAsync(mail);
            }
            catch (FormatException)
            {
                throw new ArgumentException("The provided email address is not in a valid format.", nameof(to));
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken, string userId)
        {
            // REMOVE the frontendResetPasswordUrl logic

            var subject = "Reset Your Password";
            var body = $@"
                <p>Hello {userName},</p>
                <p>We received a request to reset the password for your account.</p>
                <p>To reset your password, please go to your application's password reset page (e.g., your login page or a dedicated password reset page) and enter the following information:</p>
                <ul>
                    <li><strong>Your Email:</strong> {toEmail}</li>
                    <li><strong>Reset Token:</strong> <code>{resetToken}</code></li>
                </ul>
                <p>This token is valid for {_smtpSettings.Port} minutes (or whatever you configure for token expiry).</p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Thanks,<br>Your Application Team</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

    }
}
