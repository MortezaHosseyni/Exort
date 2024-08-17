using System.ComponentModel.DataAnnotations;
using HashCT_BackEnd.Data.Tools;
using Shared.Enums.Community;

namespace Domain.Entities
{
    public class Community : BaseEntity
    {
        [Required][MaxLength(80)] public string Name { get; private set; }
        [Required][MaxLength(300)] public string Description { get; private set; }

        [MaxLength(500)] public string? Image { get; private set; }
        [MaxLength(500)] public string? Banner { get; private set; }

        public int MembersCount { get; set; }

        public CommunityStatus Status { get; private set; }
        public CommunityType Type { get; private set; }

        [Required] public Dictionary<string, ICollection<CommunityPart>> Roles { get; private set; }

        [Required] public required ICollection<CommunityChannel> Channels { get; set; }

        public ICollection<CommunityMessage>? Messages { get; set; }

        public Community(string name, string description, string? image, string? banner, CommunityStatus status, CommunityType type, Dictionary<string, ICollection<CommunityPart>> roles)
        {
            // Clarify name
            Name = Sanitize.Clarify(name);

            // Clarify description
            Description = Sanitize.Clarify(description);

            // Check & clarify image
            Image = Sanitize.Clarify(image);

            // Check & clarify banner
            Banner = Sanitize.Clarify(banner);

            // Check status
            if (!Enum.IsDefined(typeof(CommunityStatus), status))
                throw new Exception("Status is invalid.");
            Status = status;

            // Check type
            if (!Enum.IsDefined(typeof(CommunityType), type))
                throw new Exception("Type is invalid.");
            Type = type;

            // Clarify roles
            Dictionary<string, ICollection<CommunityPart>> cleanRoles = new Dictionary<string, ICollection<CommunityPart>>();
            foreach (var role in roles)
            {
                cleanRoles.Add(Sanitize.Clarify(role.Key), role.Value);
            }
            Roles = cleanRoles;
        }
    }
}
