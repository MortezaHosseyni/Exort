using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Community
{
    public class CommunityRolePatchDto
    {
        [Required] public required string RoleName { get; set; }
        [Required] public required Ulid CommunityId { get; set; }
        [Required] public required Ulid MemberId { get; set; }
    }
}
