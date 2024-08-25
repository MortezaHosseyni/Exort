using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UsersCommunity
{
    public class UsersCommunityPostDto
    {
        [Required] public required Ulid CommunityId { get; set; }
    }
}
