using System.ComponentModel.DataAnnotations;
using Shared.Utilities;

namespace Domain.Entities
{
    public class CommunityPart : BaseEntity
    {
        [Required][MaxLength(225)] public string Name { get; private set; }
        [Required][MaxLength(500)] public string Description { get; private set; }

        public List<string>? Abilities { get; private set; }

        public CommunityPart(string name, string description, List<string>? abilities)
        {
            // Clarify name
            Name = Sanitize.Clarify(name);

            // Clarify description
            Description = Sanitize.Clarify(description);

            // Check and clarify abilities
            if (abilities != null && abilities.Any())
                Abilities = Sanitize.Clarify(abilities);
        }
    }
}
