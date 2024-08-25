using Domain.Repositories;

namespace Application.Services
{
    public interface IFriendService
    {
    }
    public class FriendService(IFriendRepository friend) : IFriendService
    {
        private readonly IFriendRepository _friend = friend;
    }
}
