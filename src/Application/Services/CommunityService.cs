using Domain.Repositories;

namespace Application.Services
{
    public interface ICommunityService
    {
    }
    public class CommunityService(ICommunityRepository community) : ICommunityService
    {
        private readonly ICommunityRepository _community = community;
    }
}
