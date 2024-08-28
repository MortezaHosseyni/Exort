using Application.DTOs.Community;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.Community;

namespace Application.Services
{
    public interface ICommunityService
    {
        Task<CommunityGetDto> GetCommunity(Ulid id);
        Task<List<CommunityGetDto>> GetUserCommunities(Ulid userId);
        Task<(CommunityGetDto?, bool, string)> AddCommunity(CommunityPostDto community, Ulid userId);
        Task<(CommunityGetDto?, bool, string)> UpdateCommunity(Ulid id, Ulid userId, CommunityPutDto community);
        Task<(bool, string)> JoinToCommunity(Ulid userId, Ulid communityId);
        Task<(bool, string)> LeaveTheCommunity(Ulid userId, Ulid communityId);
    }
    public class CommunityService(ICommunityRepository community, IUsersCommunityRepository usersCommunity, IUserRepository user, IFileManagerService fileManager, IMapper mapper) : ICommunityService
    {
        private readonly ICommunityRepository _community = community;
        private readonly IUsersCommunityRepository _usersCommunity = usersCommunity;
        private readonly IUserRepository _user = user;

        private readonly IFileManagerService _fileManager = fileManager;
        private readonly IMapper _mapper = mapper;

        private const int CommunityCreationQuota = 20;

        public async Task<CommunityGetDto> GetCommunity(Ulid id)
        {
            var filter = Builders<Community>.Filter.Gt(c => c.Id, id);
            var community = await _community.FindOneAsync(filter);

            return _mapper.Map<CommunityGetDto>(community);
        }

        public async Task<List<CommunityGetDto>> GetUserCommunities(Ulid userId)
        {
            var filter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId);
            var userCommunities = await _usersCommunity.FindAsync(filter);

            var communities = new List<CommunityGetDto>();

            foreach (var userCommunity in userCommunities)
            {
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, userCommunity.CommunityId) & Builders<Community>.Filter.Gt(c => c.Status, CommunityStatus.Active);
                var community = await _community.FindOneAsync(communityFilter);
                communities.Add(_mapper.Map<CommunityGetDto>(community));
            }

            return communities;
        }

        public async Task<(CommunityGetDto?, bool, string)> AddCommunity(CommunityPostDto community, Ulid userId)
        {
            try
            {
                // Check User Communities
                var checkUserCommunities = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId);
                var userCommunitiesCount = await _usersCommunity.CountByPredicateAsync(checkUserCommunities);
                if (userCommunitiesCount >= CommunityCreationQuota)
                    return (null, false, $"This User has reached Community creation quota (Maximum Community user can create or join: {CommunityCreationQuota}).");

                // Add Community
                var id = Ulid.NewUlid();
                var defaultRoles = new Dictionary<string, ICollection<CommunityPart>> { { "Member", new List<CommunityPart>()
                {
                    new ("Send Message", "User can send a message to a text channel from Community.", new List<string>())
                    {
                        CreateDateTime = DateTime.Now,
                        UpdateDateTime = DateTime.Now
                    },
                    new ("Join Voice", "User can join and connect to any voice channel from Community.", new List<string>())
                    {
                        CreateDateTime = DateTime.Now,
                        UpdateDateTime = DateTime.Now
                    }
                } } };

                var addedCommunity = new Community(community.Name, community.Description,
                    community.Image != null
                        ? _fileManager.SaveFileAndReturnName(community.Image, $"wwwroot/Communities/{id}/Images")
                        : null,
                    community.Banner != null
                        ? _fileManager.SaveFileAndReturnName(community.Banner, $"wwwroot/Communities/{id}/Banners")
                        : null,
                    CommunityStatus.Active, community.Type, defaultRoles)
                {
                    Id = id,
                    MembersCount = 1,
                    Channels = new List<CommunityChannel>()
                    {
                        new ("General Text", null, CommunityChannelType.Text, CommunityChannelStatus.Active)
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now,
                            CommunityId = id
                        },
                        new ("General Voice", null, CommunityChannelType.Voice, CommunityChannelStatus.Active)
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now,
                            CommunityId = id
                        }
                    },
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _community.AddAsync(addedCommunity);

                // Add User Community
                var addedUserCommunity = new UsersCommunity(UserCommunityStatus.Active)
                {
                    CommunityId = id,
                    UserId = userId,
                    Roles = new Dictionary<string, ICollection<CommunityPart>>()
                    {
                        {
                            "Owner", new List<CommunityPart>()
                            {
                                new("Send Message", "User can send a message to a text channel from Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Join Voice", "User can join and connect to any voice channel from Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Ban & Unban", "Ban & unban another users from Community.", new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Create Voice Channel", "User can create, update or delete a voice channel for Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Create Text Channel", "User can create, update or delete a text channel for Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Remove Messages", "User can remove message from any channel of Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new("Voice Kick", "User can kick other users from any voice channel of Community.",
                                    new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new ("Create Role", "User can create, update or delete a role in Community.", new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new ("Give Role", "User can give or remove a role to any member from Community.", new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                },
                                new ("Update Community", "User can update information about Community.", new List<string>())
                                {
                                    CreateDateTime = DateTime.Now,
                                    UpdateDateTime = DateTime.Now
                                }
                            }
                        }
                    },
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _usersCommunity.AddAsync(addedUserCommunity);

                return (_mapper.Map<CommunityGetDto>(addedCommunity), true, "Community created successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(CommunityGetDto?, bool, string)> UpdateCommunity(Ulid id, Ulid userId, CommunityPutDto community)
        {
            try
            {
                // Find & check User
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, id) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (null, false, "This User is not joined in this Community.");

                var userCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                // Check User access
                var hasAccess = userCommunity.Roles
                    .SelectMany(role => role.Value)
                    .Any(communityPart => communityPart.Name == "Update Community");
                if (!hasAccess)
                    return (null, false, "This User has not have access to update this Community.");

                // Find Community
                var filter = Builders<Community>.Filter.Gt(c => c.Id, id);
                var communityModel = await _community.FindOneAsync(filter);

                // Update Community
                var updatedCommunity = new Community(community.Name, community.Description,
                    community.Image != null
                        ? _fileManager.SaveFileAndReturnName(community.Image, $"wwwroot/Communities/{id}/Images")
                        : communityModel.Image,
                    community.Banner != null
                        ? _fileManager.SaveFileAndReturnName(community.Banner, $"wwwroot/Communities/{id}/Banners")
                        : communityModel.Banner,
                    CommunityStatus.Active, community.Type, communityModel.Roles)
                {
                    Id = id,
                    MembersCount = communityModel.MembersCount,
                    Channels = communityModel.Channels,
                    CreateDateTime = communityModel.CreateDateTime,
                    UpdateDateTime = DateTime.Now
                };

                await _community.UpdateAsync(filter, updatedCommunity);

                return (_mapper.Map<CommunityGetDto>(updatedCommunity), true, "Community updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(bool, string)> JoinToCommunity(Ulid userId, Ulid communityId)
        {
            try
            {
                var checkUserExistsFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(checkUserExistsFilter))
                    return (false, "Couldn't find any User with that Id.");

                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (false, "Couldn't find any Community with that Id.");

                var checkJoinFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) & Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId);
                if (await _usersCommunity.AnyByPredicateAsync(checkJoinFilter))
                    return (false, "User already joined in this Community.");

                var checkUserCommunities = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId);
                var userCommunitiesCount = await _usersCommunity.CountByPredicateAsync(checkUserCommunities);
                if (userCommunitiesCount >= CommunityCreationQuota)
                    return (false, $"This User has reached quota to join Communities (Maximum Community user can create or join: {CommunityCreationQuota}).");

                var addedUsersCommunity = new UsersCommunity(UserCommunityStatus.Active)
                {
                    UserId = userId,
                    CommunityId = communityId,
                    Roles = new Dictionary<string, ICollection<CommunityPart>> { { "Member", new List<CommunityPart>()
                    {
                        new ("Send Message", "User can send a message to a text channel from Community.", new List<string>())
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now
                        },
                        new ("Join Voice", "User can join and connect to any voice channel from Community.", new List<string>())
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now
                        }
                    } } },
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };
                await _usersCommunity.AddAsync(addedUsersCommunity);

                var community = await _community.FindOneAsync(communityFilter);
                community.MembersCount = community.MembersCount + 1;
                await _community.UpdateAsync(communityFilter, community);

                return (true, "User joined Community successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> LeaveTheCommunity(Ulid userId, Ulid communityId)
        {
            try
            {
                var checkUserExistsFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(checkUserExistsFilter))
                    return (false, "Couldn't find any User with that Id.");

                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
                if (!await _community.AnyByPredicateAsync(communityFilter))
                    return (false, "Couldn't find any Community with that Id.");

                var checkJoinFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) & Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId);
                if (!await _usersCommunity.AnyByPredicateAsync(checkJoinFilter))
                    return (false, "User is not registered to this Community.");

                var userCommunity = await _usersCommunity.FindOneAsync(checkJoinFilter);
                var community = await _community.FindOneAsync(communityFilter);
                if (userCommunity.Roles.ContainsKey("Owner"))
                {
                    community.MembersCount = community.MembersCount - 1;
                    community.UpdateStatus(CommunityStatus.Deleted);
                    await _community.UpdateAsync(communityFilter, community);
                }
                else
                {
                    community.MembersCount = community.MembersCount - 1;
                    await _community.UpdateAsync(communityFilter, community);
                }

                await _usersCommunity.RemoveAsync(checkJoinFilter);

                return (true, "User leaved Community successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
