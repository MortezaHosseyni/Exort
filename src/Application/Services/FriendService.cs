using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.User;

namespace Application.Services
{
    public interface IFriendService
    {
        Task<List<UserGetDto>> GetFriends(Ulid userId);
        Task<List<UserGetDto>> GetMutualFriends(Ulid userId, Ulid friendId);
        Task<(bool, string)> AddFriend(Ulid userId, Ulid friendId);
        Task<(bool, string)> RemoveFriend(Ulid id, Ulid userId, Ulid friendId);
        Task<(bool, string)> AcceptFriend(Ulid userId, Ulid id);
    }
    public class FriendService(IFriendRepository friend, IUserRepository user, IMapper mapper) : IFriendService
    {
        private readonly IFriendRepository _friend = friend;
        private readonly IUserRepository _user = user;

        private readonly IMapper _mapper = mapper;

        public async Task<List<UserGetDto>> GetFriends(Ulid userId)
        {
            var filter = Builders<Friend>.Filter.Gt(f => f.UserId, userId) &
                         Builders<Friend>.Filter.Gt(f => f.Status, FriendStatus.Active);
            var userFriends = await _friend.FindAsync(filter);

            var friends = new List<UserGetDto>();

            foreach (var friend in userFriends)
            {
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, friend.FriendId) & Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                friends.Add(_mapper.Map<UserGetDto>(await _user.FindOneAsync(userFilter)));
            }

            return friends;
        }

        public async Task<List<UserGetDto>> GetMutualFriends(Ulid userId, Ulid friendId)
        {
            var mutualFriendsId = await _friend.GetMutualFriends(userId.ToString(), friendId.ToString());
            var friends = new List<UserGetDto>();
            foreach (var userFilter in mutualFriendsId.Select(friend => Builders<User>.Filter.Gt(u => u.Id, friend) & Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active)))
            {
                var user = await _user.FindOneAsync(userFilter);
                friends.Add(_mapper.Map<UserGetDto>(user));
            }

            return friends;
        }

        public async Task<(bool, string)> AddFriend(Ulid userId, Ulid friendId)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id");

                // Check Friend
                var friendFilter = Builders<User>.Filter.Gt(u => u.Id, friendId);
                if (!await _user.AnyByPredicateAsync(friendFilter))
                    return (false, "Couldn't find any User with that Id for add to friend list.");

                // Add Friend
                var invitedFriend = new Friend(FriendStatus.Invited)
                {
                    UserId = userId,
                    FriendId = friendId,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _friend.AddAsync(invitedFriend);

                return (true, "Invite sent to friend successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> RemoveFriend(Ulid id, Ulid userId, Ulid friendId)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id");

                // Check Friend
                var friendFilter = Builders<User>.Filter.Gt(u => u.Id, friendId);
                if (!await _user.AnyByPredicateAsync(friendFilter))
                    return (false, "Couldn't find any User with that Id for add to friend list.");

                // Remove Friend
                var filter = Builders<Friend>.Filter.Gt(f => f.Id, id) &
                             Builders<Friend>.Filter.Gt(f => f.UserId, userId) &
                             Builders<Friend>.Filter.Gt(f => f.FriendId, friendId);
                if (!await _friend.AnyByPredicateAsync(filter))
                    return (false, "Couldn't find any friend with that Ids.");

                await _friend.RemoveAsync(filter);

                return (true, "Friend removed successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> AcceptFriend(Ulid userId, Ulid id)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id");

                // Accept Invite
                var filter = Builders<Friend>.Filter.Gt(f => f.Id, id) &
                             Builders<Friend>.Filter.Gt(f => f.FriendId, userId);
                var friend = await _friend.FindOneAsync(filter);
                friend.UpdateStatus(FriendStatus.Active);
                await _friend.UpdateAsync(filter, friend);

                // Add Friend
                var addFriend = new Friend(FriendStatus.Active)
                {
                    UserId = userId,
                    FriendId = friend.UserId,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _friend.AddAsync(addFriend);

                return (true, "Friend request accepted successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
