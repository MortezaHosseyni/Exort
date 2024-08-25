using Domain.Repositories;

namespace Application.Services
{
    public interface ICommunityMessageService
    {
    }
    public class CommunityMessageService(ICommunityMessageRepository communityMessage) : ICommunityMessageService
    {
        private readonly ICommunityMessageRepository _communityMessage = communityMessage;
    }
}
