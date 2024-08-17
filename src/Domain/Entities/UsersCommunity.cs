using System.ComponentModel.DataAnnotations;
using Shared.Enums.Community;

namespace Domain.Entities
{
    public class UsersCommunity : BaseEntity
    {
        [Required] public required Ulid UserId { get; set; }
        [Required] public required Ulid CommunityId { get; set; }
        [Required] public required Dictionary<string, ICollection<CommunityPart>> Roles { get; set; }

        [Required] public UserCommunityStatus Status { get; private set; }

        public UsersCommunity(UserCommunityStatus status)
        {
            // Check status
            if (!Enum.IsDefined(typeof(UserCommunityStatus), status))
                throw new Exception("Status is invalid.");
            Status = status;
        }
    }
}
