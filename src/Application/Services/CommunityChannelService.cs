using Application.DTOs.CommunityChannel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.Community;

namespace Application.Services
{
    public interface ICommunityChannelService
    {
        Task<CommunityChannelGetDto> GetChannel(Ulid id);
        Task<(List<CommunityChannelGetDto>?, string)> GetCommunityChannels(Ulid communityId, Ulid userId);
        Task<(CommunityChannelGetDto?, bool, string)> AddCommunityChannel(Ulid userId, CommunityChannelPostDto communityChannel);
        Task<(CommunityChannelGetDto?, bool, string)> UpdateCommunityChannel(Ulid userId, Ulid channelId, CommunityChannelPutDto communityChannel);
        Task<(bool, string)> DeleteChannel(Ulid userId, Ulid channelId);
    }
    public class CommunityChannelService(ICommunityChannelRepository communityChannel, ICommunityRepository community, IUsersCommunityRepository usersCommunity, IMapper mapper) : ICommunityChannelService
    {
        private readonly ICommunityChannelRepository _communityChannel = communityChannel;
        private readonly ICommunityRepository _community = community;
        private readonly IUsersCommunityRepository _usersCommunity = usersCommunity;

        private readonly IMapper _mapper = mapper;

        private const int MaximumChannelCount = 60;

        public async Task<CommunityChannelGetDto> GetChannel(Ulid id)
        {
            var filter = Builders<CommunityChannel>.Filter.Gt(cc => cc.Id, id) & Builders<CommunityChannel>.Filter.Gt(cc => cc.Status, CommunityChannelStatus.Active);
            var communityChannel = await _communityChannel.FindOneAsync(filter);

            return _mapper.Map<CommunityChannelGetDto>(communityChannel);
        }

        public async Task<(List<CommunityChannelGetDto>?, string)> GetCommunityChannels(Ulid communityId, Ulid userId)
        {
            // Check User registration
            var userFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                             Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId);
            if (!await _usersCommunity.AnyByPredicateAsync(userFilter))
                return (null, "This User is not member of this Community.");

            var filter = Builders<CommunityChannel>.Filter.Gt(cc => cc.CommunityId, communityId) & Builders<CommunityChannel>.Filter.Gt(cc => cc.Status, CommunityChannelStatus.Active);
            var channels = await _communityChannel.FindAsync(filter);

            return (_mapper.Map<List<CommunityChannelGetDto>>(channels), string.Empty);
        }

        public async Task<(CommunityChannelGetDto?, bool, string)> AddCommunityChannel(Ulid userId, CommunityChannelPostDto communityChannel)
        {
            try
            {
                // Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityChannel.CommunityId);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (null, false, "Couldn't find any Community with that Id.");
                var community = await _community.FindOneAsync(communityFilter);
                if (community.Status != CommunityStatus.Active)
                    return (null, false, "This Community is not active.");
                if (community.Channels.Count(x => x.Status != CommunityChannelStatus.Deleted) >= MaximumChannelCount)
                    return (null, false, "This Community is reached channel quota.");

                // Check User registration
                var userFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                 Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityChannel.CommunityId);
                if (!await _usersCommunity.AnyByPredicateAsync(userFilter))
                    return (null, false, "This User is not member of this Community.");

                // Check User access
                var user = await _usersCommunity.FindOneAsync(userFilter);

                if (communityChannel.Type == CommunityChannelType.Voice && !user.Roles.ContainsKey("Create Voice Channel"))
                    return (null, false, "This User cannot be create a voice channel in this Community (access denied).");

                if (communityChannel.Type == CommunityChannelType.Text && !user.Roles.ContainsKey("Create Text Channel"))
                    return (null, false, "This User cannot be create a text channel in this Community (access denied).");

                // Create channel
                var addedCommunityChannel = new CommunityChannel(communityChannel.Title, communityChannel.Description, communityChannel.Type, CommunityChannelStatus.Active)
                {
                    CommunityId = communityChannel.CommunityId,
                    Index = communityChannel.Index,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _communityChannel.AddAsync(addedCommunityChannel);

                return (_mapper.Map<CommunityChannelGetDto>(addedCommunityChannel), true, "Channel created successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(CommunityChannelGetDto?, bool, string)> UpdateCommunityChannel(Ulid userId, Ulid channelId,
            CommunityChannelPutDto communityChannel)
        {
            try
            {
                // Find channel
                var channelFilter = Builders<CommunityChannel>.Filter.Gt(cc => cc.Id, channelId);
                if (!await _communityChannel.AnyByPredicateAsync(channelFilter))
                    return (null, false, "Couldn't find any Channel with that Id.");

                var channel = await _communityChannel.FindOneAsync(channelFilter);

                // Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, channel.CommunityId);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (null, false, "Couldn't find any Community with that Id.");
                var community = await _community.FindOneAsync(communityFilter);
                if (community.Status != CommunityStatus.Active)
                    return (null, false, "This Community is not active.");

                // Check User registration
                var userFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                 Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, channel.CommunityId);
                if (!await _usersCommunity.AnyByPredicateAsync(userFilter))
                    return (null, false, "This User is not member of this Community.");

                // Check User access
                var user = await _usersCommunity.FindOneAsync(userFilter);

                if (channel.Type == CommunityChannelType.Voice && !user.Roles.ContainsKey("Create Voice Channel"))
                    return (null, false, "This User cannot be edit a voice channel in this Community (access denied).");

                if (channel.Type == CommunityChannelType.Text && !user.Roles.ContainsKey("Create Text Channel"))
                    return (null, false, "This User cannot be edit a text channel in this Community (access denied).");

                // Update channel
                var updatedChannel =
                    new CommunityChannel(communityChannel.Title, communityChannel.Description, channel.Type, channel.Status)
                    {
                        Id = channel.Id,
                        Index = communityChannel.Index,
                        CommunityId = channel.CommunityId,
                        CreateDateTime = channel.CreateDateTime,
                        UpdateDateTime = DateTime.Now
                    };
                await _communityChannel.UpdateAsync(channelFilter, updatedChannel);

                return (_mapper.Map<CommunityChannelGetDto>(updatedChannel), true, "Channel updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(bool, string)> DeleteChannel(Ulid userId, Ulid channelId)
        {
            try
            {
                // Find channel
                var channelFilter = Builders<CommunityChannel>.Filter.Gt(cc => cc.Id, channelId);
                if (!await _communityChannel.AnyByPredicateAsync(channelFilter))
                    return (false, "Couldn't find any Channel with that Id.");

                var channel = await _communityChannel.FindOneAsync(channelFilter);

                if (channel.Status == CommunityChannelStatus.Deleted)
                    return (false, "This channel already deleted.");

                // Check User registration
                var userFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                 Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, channel.CommunityId);
                if (!await _usersCommunity.AnyByPredicateAsync(userFilter))
                    return (false, "This User is not member of this Community.");

                // Check User access
                var user = await _usersCommunity.FindOneAsync(userFilter);

                if (channel.Type == CommunityChannelType.Voice && !user.Roles.ContainsKey("Create Voice Channel"))
                    return (false, "This User cannot be delete a voice channel in this Community (access denied).");

                if (channel.Type == CommunityChannelType.Text && !user.Roles.ContainsKey("Create Text Channel"))
                    return (false, "This User cannot be delete a text channel in this Community (access denied).");

                // Delete channel
                channel.UpdateStatus(CommunityChannelStatus.Deleted);
                await _communityChannel.UpdateAsync(channelFilter, channel);

                return (true, "Channel deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
