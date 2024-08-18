using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommunityRepository
    {
        Task<Community> Add(Community community);
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
    }
}
