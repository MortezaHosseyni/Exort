using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUsersCommunityRepository : IGenericRepository<UsersCommunity> { }
    public class UsersCommunityRepository(AppDatabase database)
        : GenericRepository<UsersCommunity>(database.UC), IUsersCommunityRepository;
}
