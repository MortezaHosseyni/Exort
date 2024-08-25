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
        Task<(CommunityGetDto?, string)> AddCommunity(CommunityPostDto community);
        Task<(CommunityGetDto?, string)> UpdateCommunity(Ulid id, CommunityPutDto community);
    }
    public class CommunityService(ICommunityRepository community, IFileManagerService fileManager, IMapper mapper) : ICommunityService
    {
        private readonly ICommunityRepository _community = community;

        private readonly IFileManagerService _fileManager = fileManager;
        private readonly IMapper _mapper = mapper;

        public async Task<CommunityGetDto> GetCommunity(Ulid id)
        {
            var filter = Builders<Community>.Filter.Gt(c => c.Id, id);
            var community = await _community.FindOneAsync(filter);

            return _mapper.Map<CommunityGetDto>(community);
        }

        public async Task<(CommunityGetDto?, string)> AddCommunity(CommunityPostDto community)
        {
            try
            {
                var id = Ulid.NewUlid();
                var defaultRoles = new Dictionary<string, ICollection<CommunityPart>> { { "Member", new List<CommunityPart>() } }; // TODO: Seed default 'Community Parts' and use them.

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
                    MembersCount = 1, // TODO: Add community owner as a member
                    Channels = new List<CommunityChannel>()
                    {
                        new ("General Text", null, CommunityChannelType.Text)
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now
                        },
                        new ("General Voice", null, CommunityChannelType.Voice)
                        {
                            CreateDateTime = DateTime.Now,
                            UpdateDateTime = DateTime.Now
                        }
                    },
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };

                await _community.AddAsync(addedCommunity);

                return (_mapper.Map<CommunityGetDto>(addedCommunity), "Community created successfully.");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(CommunityGetDto?, string)> UpdateCommunity(Ulid id, CommunityPutDto community)
        {
            try
            {
                var filter = Builders<Community>.Filter.Gt(c => c.Id, id);
                var communityModel = await _community.FindOneAsync(filter);

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

                return (_mapper.Map<CommunityGetDto>(updatedCommunity), "Community updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }
}
