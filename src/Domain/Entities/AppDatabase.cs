using MongoDB.Driver;

namespace Domain.Entities
{
    public class AppDatabase
    {
        private readonly IMongoDatabase _database;

        public AppDatabase(IConfiguration configuration)
        {
            // MongoDB connection setting
            var connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(configuration["DatabaseName"]);
        }

        // User collections
        public IMongoCollection<User> U =>
            _database.GetCollection<User>("Users");

        #region Community
        // Community collections
        public IMongoCollection<Community> C =>
            _database.GetCollection<Community>("Communities");

        // Community Part collections
        public IMongoCollection<CommunityPart> CP =>
            _database.GetCollection<CommunityPart>("CommunityParts");

        // Community Channel collections
        public IMongoCollection<CommunityChannel> CC =>
            _database.GetCollection<CommunityChannel>("CommunityChannels");

        // Community Message collections
        public IMongoCollection<CommunityMessage> CM =>
            _database.GetCollection<CommunityMessage>("CommunityMessages");

        // Users Community collections
        public IMongoCollection<UsersCommunity> UC =>
            _database.GetCollection<UsersCommunity>("UsersCommunities");
        #endregion

        #region Friends
        // Friends collections
        public IMongoCollection<Friend> F =>
            _database.GetCollection<Friend>("Friends");

        // Private Messages collections
        public IMongoCollection<PrivateMessage> PM =>
            _database.GetCollection<PrivateMessage>("PrivateMessages");
        #endregion
    }
}