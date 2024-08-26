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
        Task<(bool, string)> RemoveFriend(Ulid userId, Ulid friendId);
        Task<(bool, string)> AcceptFriend(Ulid userId, Ulid friendId);
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

        }

        public async Task<(bool, string)> AddFriend(Ulid userId, Ulid friendId)
        {

        }

        public async Task<(bool, string)> RemoveFriend(Ulid userId, Ulid friendId)
        {

        }

        public async Task<(bool, string)> AcceptFriend(Ulid userId, Ulid friendId)
        {

        }
    }
}
