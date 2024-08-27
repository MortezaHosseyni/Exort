using Application.DTOs.Community;
using Application.DTOs.CommunityPart;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using System.Globalization;
using Shared.Enums.Community;

namespace Application.Services
{
    public interface ICommunityRolesService
    {
        Task<Dictionary<string, List<CommunityPartGetDto>>> GetCommunityRoles(Ulid communityId);
        Task<(bool, string)> AddRoleToCommunity(Ulid userId, CommunityRolePostDto role);
        Task<(bool, string)> RemoveRoleFromCommunity(string roleName, Ulid userId, Ulid communityId);
        Task<(bool, string)> GiveRoleToUser(string roleName, Ulid userId, Ulid communityId, Ulid memberId);
        Task<(bool, string)> RemoveUserRole(string roleName, Ulid userId, Ulid communityId, Ulid memberId);
    }
    public class CommunityRolesService(IUsersCommunityRepository usersCommunity, ICommunityRepository community, IUserRepository user, IMapper mapper) : ICommunityRolesService
    {
        private readonly IUsersCommunityRepository _usersCommunity = usersCommunity;
        private readonly ICommunityRepository _community = community;
        private readonly IUserRepository _user = user;

        private readonly IMapper _mapper = mapper;

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Convert the entire string to lowercase
            string lowerCased = input.ToLower();

            // Capitalize the first letter of the first word
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string result = textInfo.ToTitleCase(lowerCased);

            return result;
        }

        public async Task<Dictionary<string, List<CommunityPartGetDto>>> GetCommunityRoles(Ulid communityId)
        {
            var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
            var community = await _community.FindOneAsync(communityFilter);

            return community.Roles.ToDictionary(communityRole => communityRole.Key, communityRole => _mapper.Map<List<CommunityPartGetDto>>(communityRole.Value));
        }

        public async Task<(bool, string)> AddRoleToCommunity(Ulid userId, CommunityRolePostDto role)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Find & Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, role.CommunityId);
                var community = await _community.FindOneAsync(communityFilter);

                // Check User in Community
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, role.CommunityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");

                // Check User access in Community
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                var hasAccess = userInCommunity.Roles
                    .SelectMany(r => r.Value)
                    .Any(communityPart => communityPart.Name == "Create Role");
                if (!hasAccess)
                    return (false, "This User is not have access to create role in this Community.");

                // Add Role
                community.Roles.Add(ToTitleCase(role.Name), _mapper.Map<List<CommunityPart>>(role.Permissions));
                await _community.UpdateAsync(communityFilter, community);

                return (true, "Role created for this Community successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> RemoveRoleFromCommunity(string roleName, Ulid userId, Ulid communityId)
        {
            try
            {
                // Check Role Name
                if (ToTitleCase(roleName).Contains("Owner") || ToTitleCase(roleName).Contains("Member"))
                    return (false, "You cannot remove Owner or Member roles!");

                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Find & Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
                var community = await _community.FindOneAsync(communityFilter);

                // Check User in Community
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");

                // Check User access in Community
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                var hasAccess = userInCommunity.Roles
                    .SelectMany(r => r.Value)
                    .Any(communityPart => communityPart.Name == "Create Role");
                if (!hasAccess)
                    return (false, "This User is not have access to create role in this Community.");

                // Check & Remove Role
                if (!community.Roles.ContainsKey(ToTitleCase(roleName)))
                    return (false, "There is no role with that name in this Community.");

                community.Roles.Remove(ToTitleCase(roleName));
                await _community.UpdateAsync(communityFilter, community);

                // Remove Role for all Community members
                var communityMembersFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId) &
                                             Builders<UsersCommunity>.Filter.ElemMatch(uc => uc.Roles, role => role.Key == ToTitleCase(roleName));
                var members = await _usersCommunity.FindAsync(communityMembersFilter);
                foreach (var member in members)
                {
                    if (member.Roles.ContainsKey(ToTitleCase(roleName)))
                        member.Roles.Remove(ToTitleCase(roleName));
                }

                return (true, "Role removed from this Community and from all member of this Community successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> GiveRoleToUser(string roleName, Ulid userId, Ulid communityId, Ulid memberId)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Find & Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
                var community = await _community.FindOneAsync(communityFilter);

                // Check User in Community
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");

                // Check User access in Community
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                var hasAccess = userInCommunity.Roles
                    .SelectMany(r => r.Value)
                    .Any(communityPart => communityPart.Name == "Give Role");
                if (!hasAccess)
                    return (false, "This User is not have access to create role in this Community.");

                // Check Role Name
                if (ToTitleCase(roleName).Contains("Owner"))
                    return (false, "You cannot give Owner role to another User!");

                // Check Role in Community
                if (!community.Roles.ContainsKey(ToTitleCase(roleName)))
                    return (false, "This Role is not registered in this Community!");

                // Find member & give Role
                var memberFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, memberId) &
                                   Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId);
                var member = await _usersCommunity.FindOneAsync(memberFilter);

                var role = community.Roles.FirstOrDefault(r => r.Key == ToTitleCase(roleName));
                member.Roles.Add(ToTitleCase(role.Key), role.Value);
                await _usersCommunity.UpdateAsync(memberFilter, member);

                return (true, "Role assigned to member successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> RemoveUserRole(string roleName, Ulid userId, Ulid communityId, Ulid memberId)
        {
            try
            {
                // Check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, userId);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                // Find & Check Community
                var communityFilter = Builders<Community>.Filter.Gt(c => c.Id, communityId);
                var community = await _community.FindOneAsync(communityFilter);

                // Check User in Community
                var userCommunityFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, userId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId) &
                                          Builders<UsersCommunity>.Filter.Gt(uc => uc.Status, UserCommunityStatus.Active);
                if (!await _usersCommunity.AnyByPredicateAsync(userCommunityFilter))
                    return (false, "This User is not joined in this Community.");

                // Check User access in Community
                var userInCommunity = await _usersCommunity.FindOneAsync(userCommunityFilter);

                var hasAccess = userInCommunity.Roles
                    .SelectMany(r => r.Value)
                    .Any(communityPart => communityPart.Name == "Give Role");
                if (!hasAccess)
                    return (false, "This User is not have access to create role in this Community.");

                // Check Role Name
                if (ToTitleCase(roleName).Contains("Owner"))
                    return (false, "You cannot give Owner role to another User!");

                // Check Role in Community
                if (!community.Roles.ContainsKey(ToTitleCase(roleName)))
                    return (false, "This Role is not registered in this Community!");

                // Find member & remove Role
                var memberFilter = Builders<UsersCommunity>.Filter.Gt(uc => uc.UserId, memberId) &
                                   Builders<UsersCommunity>.Filter.Gt(uc => uc.CommunityId, communityId);
                var member = await _usersCommunity.FindOneAsync(memberFilter);

                member.Roles.Remove(ToTitleCase(roleName));
                await _usersCommunity.UpdateAsync(memberFilter, member);

                return (true, "Role removed for member successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
