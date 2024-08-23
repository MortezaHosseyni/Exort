using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommunityChannelRepository : IGenericRepository<CommunityChannel> { }
    public class CommunityChannelRepository(AppDatabase database)
        : GenericRepository<CommunityChannel>(database.CC), ICommunityChannelRepository;
}
