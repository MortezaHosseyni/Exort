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
            var database = JsonSerializer.Deserialize<EmailSettings>(jsonContent);

            return database!;
        }
    }
}
