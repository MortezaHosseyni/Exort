using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository : IGenericRepository<User> { }
    public class UserRepository(AppDatabase database)
        : GenericRepository<User>(database.U), IUserRepository;
}
