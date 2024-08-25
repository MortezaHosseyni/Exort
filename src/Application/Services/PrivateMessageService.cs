using Domain.Repositories;

namespace Application.Services
{
    public interface IPrivateMessageService
    {
    }
    public class PrivateMessageService(IPrivateMessageRepository privateMessage) : IPrivateMessageService
    {
        private readonly IPrivateMessageRepository _privateMessage = privateMessage;
    }
}
