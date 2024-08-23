using Domain.Entities;

namespace Domain.Repositories
{
    public interface ICommunityRepository : IGenericRepository<Community> {}
    public class CommunityRepository(AppDatabase database)
        : GenericRepository<Community>(database.C), ICommunityRepository;
}
