using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommunityPartRepository : IGenericRepository<CommunityPart> { }
    public class CommunityPartRepository(AppDatabase database)
        : GenericRepository<CommunityPart>(database.CP), ICommunityPartRepository;
}
