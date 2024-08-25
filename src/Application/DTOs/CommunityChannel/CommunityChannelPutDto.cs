using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CommunityChannel
{
    public class CommunityChannelPutDto
    {
        [Required] public int Index { get; set; }
        [Required][MaxLength(100)] public required string Title { get; set; }
        [MaxLength(225)] public string? Description { get; set; }
    }
}
