using Application.DTOs.PrivateMessage;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.User;

namespace Application.Services
{
    public interface IPrivateMessageService
    {
        Task<List<PrivateMessageGetDto>> GetFriendMessages(Ulid userId, Ulid friendId, DateTime? beforeTimestamp, int limit = 20);
        Task<(bool, string)> AddMessage(Ulid userId, PrivateMessagePostDto message);
        Task<(bool, string)> RemoveMessage(Ulid userId, Ulid messageId);
    }
    public class PrivateMessageService(IPrivateMessageRepository privateMessage, IFriendRepository friend, IUserRepository user, IMapper mapper) : IPrivateMessageService
    {
        private readonly IPrivateMessageRepository _privateMessage = privateMessage;
        private readonly IFriendRepository _friend = friend;
        private readonly IUserRepository _user = user;

        private readonly IMapper _mapper = mapper;

        public async Task<List<PrivateMessageGetDto>> GetFriendMessages(Ulid userId, Ulid friendId, DateTime? beforeTimestamp, int limit = 20)
        {
            var filter = Builders<PrivateMessage>.Filter.Eq(m => m.SenderId, userId) & Builders<PrivateMessage>.Filter.Eq(m => m.ReceiverId, friendId);

            if (beforeTimestamp.HasValue)
            {
                filter = Builders<PrivateMessage>.Filter.And(
                    filter,
                    Builders<PrivateMessage>.Filter.Lt(m => m.CreateDateTime, beforeTimestamp.Value)
                );
            }

            var messages = await _privateMessage.GetLimitedMessages(filter, limit);

            return _mapper.Map<List<PrivateMessageGetDto>>(messages);
        }

        public async Task<(bool, string)> AddMessage(Ulid userId, PrivateMessagePostDto message)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId) &
                                 Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Check Receiver
                var receiverFilter = Builders<User>.Filter.Gt(u => u.Id, message.ReceiverId) &
                                 Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(receiverFilter))
                    return (false, "Couldn't find any User with that Id for receive this message.");

                // Check friendship
                var friendFilter = Builders<Friend>.Filter.Gt(f => f.UserId, userId) &
                                   Builders<Friend>.Filter.Gt(f => f.FriendId, message.ReceiverId);
                if (!await _friend.AnyByPredicateAsync(friendFilter))
                    return (false, "This User is not your friend! (you can just send message to your friends).");

                // Add Message
                var addedMessage = new PrivateMessage()
                {
                    SenderId = userId,
                    ReceiverId = message.ReceiverId,
                    Text = message.Text,
                    Reply = message.Reply,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _privateMessage.AddAsync(addedMessage);

                return (true, "Message sent successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> RemoveMessage(Ulid userId, Ulid messageId)
        {
            try
            {
                // Find & Check message
                var messageFilter = Builders<PrivateMessage>.Filter.Gt(m => m.Id, messageId);
                if (!await _privateMessage.AnyByPredicateAsync(messageFilter))
                    return (false, "Couldn't find any message with that Id.");
                var message = await _privateMessage.FindOneAsync(messageFilter);

                // Check Sender
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId) &
                                 Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Check Receiver
                var receiverFilter = Builders<User>.Filter.Gt(u => u.Id, message.ReceiverId) &
                                     Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(receiverFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Check friendship
                var friendFilter = Builders<Friend>.Filter.Gt(f => f.UserId, userId) &
                                   Builders<Friend>.Filter.Gt(f => f.FriendId, message.ReceiverId);
                if (!await _friend.AnyByPredicateAsync(friendFilter))
                    return (false, "This User is not your friend! (you can just send message to your friends).");

                // Check sender access
                if (message.SenderId == userId)
                {
                    await _privateMessage.RemoveAsync(messageFilter);
                    return (true, "Message removed successfully.");
                }
                else
                {
                    return (false, "This message is not yours! (you can just remove your own messages).");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
