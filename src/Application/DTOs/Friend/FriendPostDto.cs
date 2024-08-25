using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Friend
{
    public class FriendPostDto
    {
        [Required] public required Ulid FriendId { get; set; }
    }
}
