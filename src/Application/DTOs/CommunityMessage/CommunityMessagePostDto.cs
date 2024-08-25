using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CommunityMessage
{
    public class CommunityMessagePostDto
    {
        [Required][MaxLength(1000)][DataType(DataType.MultilineText)] public required string Text { get; set; }

        [Required] public required Ulid CommunityId { get; set; }
        [Required] public required Ulid CommunityChannelId { get; set; }
        public Ulid? Reply { get; set; }
    }
}
