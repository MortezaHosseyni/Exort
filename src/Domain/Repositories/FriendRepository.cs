using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Domain.Repositories
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        Task<List<Ulid>> GetMutualFriends(string userId1, string userId2);
    }

    public class FriendRepository(AppDatabase database)
        : GenericRepository<Friend>(database.F), IFriendRepository
    {
        public async Task<List<Ulid>> GetMutualFriends(string userId1, string userId2)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("$or", new BsonArray
                {
                    new BsonDocument("UserId", userId1),
                    new BsonDocument("UserId", userId2)
                })),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$FriendId" },
                    { "UserIds", new BsonDocument("$addToSet", "$UserId") }
                }),
                new BsonDocument("$match", new BsonDocument("UserIds", new BsonDocument
                {
                    { "$all", new BsonArray { userId1, userId2 } }
                }))
            };

            var result = await database.F.Aggregate<BsonDocument>(pipeline).ToListAsync();

            var mutualFriends = result.ConvertAll(f => Ulid.Parse(f["_id"].ToString()));

            return mutualFriends;
        }
    }
}
