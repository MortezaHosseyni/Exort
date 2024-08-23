using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPrivateMessageRepository : IGenericRepository<PrivateMessage> { }
    public class PrivateMessageRepository(AppDatabase database)
        : GenericRepository<PrivateMessage>(database.PM), IPrivateMessageRepository;
}
