using System.ComponentModel.DataAnnotations;
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

        public Dictionary<string, List<string>> Roles { get; private set; }
    }
}
