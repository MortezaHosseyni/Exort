using System.Net;
using System.Net.Mail;
using Infrastructure.Configurations;

namespace Infrastructure.Adapters.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message, bool isHtml);
    }
    public class EmailService(IEmailConfiguration emailConfiguration) : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration = emailConfiguration;

        public async Task SendEmailAsync(string toEmail, string subject, string message, bool isHtml)
        {
            var emailSettings = _emailConfiguration.GetEmailSettings();

            using var client = new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort);
            client.Credentials = new NetworkCredential(emailSettings.SmtpUser, emailSettings.SmtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings.SenderEmail, emailSettings.SenderName),
                Subject = subject,
                Body = message,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
