using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CommunityMessage : BaseEntity
    {
        [Required][MaxLength(1000)][DataType(DataType.MultilineText)] public required string Text { get; set; }

        [Required] public required Ulid SenderId { get; set; }

        [Required] public required Ulid CommunityId { get; set; }
        [Required] public required Ulid CommunityChannelId { get; set; }
        public Ulid? Reply { get; set; }
    }
}
