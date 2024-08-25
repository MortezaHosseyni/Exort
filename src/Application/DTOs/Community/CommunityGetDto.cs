using Shared.Enums.Community;

namespace Application.DTOs.Community
{
    public class CommunityGetDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Banner { get; set; }
        public int MembersCount { get; set; }
        public CommunityType Type { get; set; }
    }
}
