using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CommunityMessage
{
    public class CommunityMessagePutDto
    {
        [Required][MaxLength(1000)][DataType(DataType.MultilineText)] public required string Text { get; set; }
    }
}
