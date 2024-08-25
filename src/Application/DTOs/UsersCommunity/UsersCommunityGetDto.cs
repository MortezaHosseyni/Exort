using Application.DTOs.CommunityPart;
using Shared.Enums.Community;

namespace Application.DTOs.UsersCommunity
{
    public class UsersCommunityGetDto
    {
        public Ulid UserId { get; set; }
        public Ulid CommunityId { get; set; }
        public Dictionary<string, ICollection<CommunityPartGetDto>>? Roles { get; set; }
        public UserCommunityStatus Status { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
