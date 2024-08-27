using Domain.Entities;
using MongoDB.Driver;

namespace Domain.Repositories
{
    public interface IPrivateMessageRepository : IGenericRepository<PrivateMessage>
    {
        Task<List<PrivateMessage>> GetLimitedMessages(FilterDefinition<PrivateMessage> filter, int limit);
    }

    public class PrivateMessageRepository(AppDatabase database)
        : GenericRepository<PrivateMessage>(database.PM), IPrivateMessageRepository
    {
        public async Task<List<PrivateMessage>> GetLimitedMessages(FilterDefinition<PrivateMessage> filter, int limit)
        {
            return await database.PM.Find(filter)
                .SortByDescending(m => m.CreateDateTime)
                .Limit(limit)
                .ToListAsync();
        }
    }
}
