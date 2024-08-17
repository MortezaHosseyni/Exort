using HashCT_BackEnd.Data.Tools;
using Shared.Enums.Community;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class CommunityChannel : BaseEntity
    {
        [Required] public int Index { get; set; }
        [Required][MaxLength(100)] public string Title { get; private set; }
        [MaxLength(225)] public string? Description { get; private set; }

        [Required] public CommunityChannelType Type { get; private set; }

        public ICollection<CommunityMessage>? Messages { get; set; }

        public CommunityChannel(string title, string? description, CommunityChannelType type)
        {
            // Clarify title
            Title = Sanitize.Clarify(title);

            // Clarify description
            Description = Sanitize.Clarify(description);

            // Check type
            if (!Enum.IsDefined(typeof(CommunityChannelType), type))
                throw new Exception("Type is invalid.");
            Type = type;
        }
    }
}
