using Domain.Entities;
using MongoDB.Driver;

namespace Domain.Repositories
{
    public interface ICommunityRepository
    {
        Task<Community> Add(Community community);
        Task<Community> Edit(Ulid id, Community community);
    }
    public class CommunityRepository : ICommunityRepository
    {
        private readonly AppDatabase _collection;

        public CommunityRepository(AppDatabase context)
        {
            _collection = context;
        }


        public async Task<Community> Add(Community community)
        {
            if (community == null) throw new ArgumentNullException("Information is invalid.");

            await _collection.C.InsertOneAsync(community);

            return community;
        }

        public async Task<Community> Edit(Ulid id, Community community)
        {
            if (id == Ulid.Empty) throw new ArgumentNullException("Information is invalid.");

            await _collection.C.ReplaceOneAsync(Builders<Community>.Filter.Eq("Id", id), community);

            return community;
        }
    }
}
