using System.ComponentModel.DataAnnotations;
using Application.DTOs.CommunityPart;

namespace Application.DTOs.Community
{
    public class CommunityRolePostDto
    {
        [Required][MaxLength(225)] public required string Name { get; set; }
        [Required] public required List<CommunityPartPostDto> Permissions { get; set; }

        [Required] public required Ulid CommunityId { get; set; }
    }
}
