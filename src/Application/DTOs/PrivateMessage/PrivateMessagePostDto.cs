using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PrivateMessage
{
    public class PrivateMessagePostDto
    {
        [Required][MaxLength(1000)][DataType(DataType.MultilineText)] public required string Text { get; set; }
        [Required] public required Ulid ReceiverId { get; set; }
        public Ulid? Reply { get; set; }
    }
}
