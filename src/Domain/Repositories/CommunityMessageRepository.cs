using Domain.Entities;
using MongoDB.Driver;

namespace Domain.Repositories
{
    public interface ICommunityMessageRepository : IGenericRepository<CommunityMessage>
    {
        Task<List<CommunityMessage>> GetLimitedMessages(FilterDefinition<CommunityMessage> filter, int limit);
    }

    public class CommunityMessageRepository(AppDatabase database)
        : GenericRepository<CommunityMessage>(database.CM), ICommunityMessageRepository
    {
        public async Task<List<CommunityMessage>> GetLimitedMessages(FilterDefinition<CommunityMessage> filter, int limit)
        {
            return await database.CM.Find(filter)
                .SortByDescending(m => m.CreateDateTime)
                .Limit(limit)
                .ToListAsync();
        }
    }
}
