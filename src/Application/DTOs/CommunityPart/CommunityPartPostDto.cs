using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CommunityPart
{
    public class CommunityPartPostDto
    {
        [Required][MaxLength(225)] public required string Name { get; set; }
        [Required][MaxLength(500)] public required string Description { get; set; }

        public List<string>? Abilities { get; set; }
    }
}
