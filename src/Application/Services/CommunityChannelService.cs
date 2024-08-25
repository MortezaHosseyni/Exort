using Domain.Repositories;

namespace Application.Services
{
    public interface ICommunityChannelService
    {
    }
    public class CommunityChannelService(ICommunityChannelRepository communityChannel) : ICommunityChannelService
    {
        private readonly ICommunityChannelRepository _communityChannel = communityChannel;
    }
}
