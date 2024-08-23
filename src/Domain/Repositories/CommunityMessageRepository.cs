using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommunityMessageRepository : IGenericRepository<CommunityMessage> { }
    public class CommunityMessageRepository(AppDatabase database)
        : GenericRepository<CommunityMessage>(database.CM), ICommunityMessageRepository;
}
