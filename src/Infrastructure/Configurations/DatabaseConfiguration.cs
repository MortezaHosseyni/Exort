using System.Text.Json;
using Shared.Formats;

namespace Infrastructure.Configurations
{
    public interface IDatabaseConfiguration
    {
        DatabaseInformation GetDatabaseInformation();
    }
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public DatabaseInformation GetDatabaseInformation()
        {
            var jsonContent = File.ReadAllText("database.json");
            var database = JsonSerializer.Deserialize<DatabaseInformation>(jsonContent);

            return database!;
        }
    }
}
