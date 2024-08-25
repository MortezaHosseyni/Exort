using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PrivateMessage
{
    public class PrivateMessagePutDto
    {
        [Required][MaxLength(1000)][DataType(DataType.MultilineText)] public required string Text { get; set; }
    }
}
