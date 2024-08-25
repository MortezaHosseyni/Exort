using Shared.Enums.User;

namespace Application.DTOs.Friend
{
    public class FriendGetDto
    {
        public Ulid UserId { get; set; }
        public Ulid FriendId { get; set; }

        public FriendStatus Status { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
