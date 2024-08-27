using Domain.Entities;
using Domain.Repositories;

namespace Application.Seeds
{
    public class CommunityPartSeeds(ICommunityPartRepository communityPart)
    {
        private readonly ICommunityPartRepository _communityPart = communityPart;

        public async Task Seed()
        {
            if (!await _communityPart.AnyAsync())
            {
                var communityParts = GetCommunityParts();
                await _communityPart.AddRangeAsync(communityParts);
            }
        }

        private static List<CommunityPart> GetCommunityParts()
        {
            var partList = new List<CommunityPart>()
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
                },
                new ("Ban & Unban", "Ban & unban another users from Community.", new List<string>())
                {
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                },
                new ("Create Voice Channel", "User can create, update or delete a voice channel for Community.", new List<string>())
                {
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                },
                new ("Create Text Channel", "User can create, update or delete a text channel for Community.", new List<string>())
                {
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                },
                new ("Remove Messages", "User can remove message from any channel of Community.", new List<string>())
                {
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                },
                new ("Voice Kick", "User can kick other users from any voice channel of Community.", new List<string>())
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
                }
            };

            return partList;
        }
    }
}
