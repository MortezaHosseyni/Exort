namespace Shared.Formats
{
    public class ConnectionStrings
    {
        public required string MongoDb { get; set; }
    }
    public class DatabaseInformation
    {
        public required ConnectionStrings ConnectionStrings { get; set; }
        public required string DatabaseName { get; set; }
    }
}
