using Shared.Enums.Community;

namespace Application.DTOs.CommunityChannel
{
    public class CommunityChannelGetDto
    {
        public Ulid Id { get; set; }
        public int Index { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public CommunityChannelType Type { get; set; }
    }
}
