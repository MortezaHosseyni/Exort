using Domain.Repositories;

namespace Application.Services
{
    public interface IUsersCommunityService
    {
    }
    public class UsersCommunityService(IUsersCommunityRepository usersCommunity) : IUsersCommunityService
    {
        private readonly IUsersCommunityRepository _usersCommunity = usersCommunity;
    }
}
