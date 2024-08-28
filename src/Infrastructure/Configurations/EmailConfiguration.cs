using Shared.Formats;
using System.Text.Json;

namespace Infrastructure.Configurations
{
    public interface IEmailConfiguration
    {
        EmailSettings GetEmailSettings();
    }
    public class EmailConfiguration : IEmailConfiguration
    {
        public EmailSettings GetEmailSettings()
        {
            var jsonContent = File.ReadAllText("email.json");
            var emailSettings = JsonSerializer.Deserialize<EmailSettings>(jsonContent);

            return emailSettings!;
        }
    }
}
