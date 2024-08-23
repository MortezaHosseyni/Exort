using Domain.Entities;

namespace Domain.Repositories
{
    public interface IFriendRepository : IGenericRepository<Friend> { }
    public class FriendRepository(AppDatabase database)
        : GenericRepository<Friend>(database.F), IFriendRepository;
}
