using Application.DTOs.CommunityMessage;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.Community;
using Shared.Enums.User;

namespace Application.Services
{
    public interface ICommunityMessageService
    {
        Task<List<CommunityMessageGetDto>> GetChannelMessages(Ulid channelId, DateTime? beforeTimestamp, int limit = 20);
        Task<(bool, string)> AddMessage(Ulid userId, CommunityMessagePostDto message);
        Task<(bool, string)> RemoveMessage(Ulid userId, Ulid messageId);
    }
    public class CommunityMessageService(ICommunityMessageRepository communityMessage, ICommunityRepository community,
        ICommunityChannelRepository communityChannel, IUserRepository user,
        IUsersCommunityRepository usersCommunity, IMapper mapper) : ICommunityMessageService
    {
        private readonly ICommunityMessageRepository _communityMessage = communityMessage;
        private readonly ICommunityRepository _community = community;
        private readonly ICommunityChannelRepository _channel = communityChannel;
        private readonly IUserRepository _user = user;
        private readonly IUsersCommunityRepository _usersCommunity = usersCommunity;

        private readonly IMapper _mapper = mapper;

        public async Task<List<CommunityMessageGetDto>> GetChannelMessages(Ulid channelId, DateTime? beforeTimestamp, int limit = 20)
        {
            var filter = Builders<CommunityMessage>.Filter.Eq(m => m.CommunityChannelId, channelId);

            if (beforeTimestamp.HasValue)
            {
                filter = Builders<CommunityMessage>.Filter.And(
                    filter,
                    Builders<CommunityMessage>.Filter.Lt(m => m.CreateDateTime, beforeTimestamp.Value)
                );
            }

            var messages = await _communityMessage.GetLimitedMessages(filter, limit);

            return _mapper.Map<List<CommunityMessageGetDto>>(messages);
        }

        public async Task<(bool, string)> AddMessage(Ulid userId, CommunityMessagePostDto message)
        {
            try
            {
                // Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, message.CommunityId) &
                                      Builders<Community>.Filter.Gt(c => c.Status, CommunityStatus.Active);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (false, "Couldn't find any Community with that Id or found Community is Inactive!");

                // Check Channel
                var channelFilter = Builders<CommunityChannel>.Filter.Gt(cc => cc.Id, message.CommunityChannelId) &
                                       Builders<CommunityChannel>.Filter.Gt(cc => cc.Type, CommunityChannelType.Text) &
                                       Builders<CommunityChannel>.Filter.Gt(cc => cc.Status, CommunityChannelStatus.Active);
                if (!await _channel.AnyByPredicateAsync(channelFilter))
                    return (false, "Couldn't find any Community Text Channel with that Id or found Community Channel is Inactive!");

                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId) &
                                 Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Check User Community and Accesses
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, message.CommunityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                // Check roles and accesses
                var hasAccess = userInCommunity.Roles
                    .SelectMany(role => role.Value)
                    .Any(communityPart => communityPart.Name == "Send Message");
                if (!hasAccess)
                    return (false, "This User is not have access to send message in this Community.");

                // Add Message
                var addedMessage = new CommunityMessage()
                {
                    CommunityId = message.CommunityId,
                    CommunityChannelId = message.CommunityChannelId,
                    SenderId = userId,
                    Text = message.Text,
                    Reply = message.Reply,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _communityMessage.AddAsync(addedMessage);

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
                var messageFilter = Builders<CommunityMessage>.Filter.Gt(cm => cm.Id, messageId);
                if (!await _communityMessage.AnyByPredicateAsync(messageFilter))
                    return (false, "Couldn't find any message with that Id.");
                var message = await _communityMessage.FindOneAsync(messageFilter);

                // Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, message.CommunityId) &
                                      Builders<Community>.Filter.Gt(c => c.Status, CommunityStatus.Active);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (false, "Couldn't find any Community with that Id or found Community is Inactive!");

                // Check Channel
                var channelFilter = Builders<CommunityChannel>.Filter.Gt(cc => cc.Id, message.CommunityChannelId) &
                                    Builders<CommunityChannel>.Filter.Gt(cc => cc.Type, CommunityChannelType.Text) &
                                    Builders<CommunityChannel>.Filter.Gt(cc => cc.Status, CommunityChannelStatus.Active);
                if (!await _channel.AnyByPredicateAsync(channelFilter))
                    return (false, "Couldn't find any Community Text Channel with that Id or found Community Channel is Inactive!");

                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId) &
                                 Builders<User>.Filter.Gt(u => u.Status, UserStatus.Active);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Check User Community and Accesses
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, message.CommunityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                // Check roles and accesses
                if (message.SenderId == userId)
                {
                    await _communityMessage.RemoveAsync(messageFilter);
                    return (true, "Message removed successfully.");
                }
                else
                {
                    var hasAccess = userInCommunity.Roles
                        .SelectMany(role => role.Value)
                        .Any(communityPart => communityPart.Name == "Remove Messages");
                    if (!hasAccess)
                        return (false, "This User is not have access to remove message in this Community.");

                    await _communityMessage.RemoveAsync(messageFilter);
                    return (true, "Message removed successfully.");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
