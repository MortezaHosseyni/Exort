using System.ComponentModel.DataAnnotations;
using Shared.Enums.User;

namespace Domain.Entities
{
    public class Friend : BaseEntity
    {
        [Required] public required Ulid UserId { get; set; }
        [Required] public required Ulid FriendId { get; set; }

        [Required] public FriendStatus Status { get; private set; }

        public Friend(FriendStatus status)
        {
            // Check friend status
            if (!Enum.IsDefined(typeof(FriendStatus), status))
                throw new Exception("Friend status is invalid.");
            Status = status;
        }

        public void UpdateStatus(FriendStatus status)
        {
            // Check friend status
            if (!Enum.IsDefined(typeof(FriendStatus), status))
                throw new Exception("Friend status is invalid.");
            Status = status;
        }
    }
}
