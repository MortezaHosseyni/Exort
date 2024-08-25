using Shared.Enums.Community;
using System.ComponentModel.DataAnnotations;
using Application.DTOs.CommunityPart;

namespace Application.DTOs.UsersCommunity
{
    public class UsersCommunityPutDto
    {
        [Required] public required Dictionary<string, ICollection<CommunityPartGetDto>> Roles { get; set; }

        [Required] public UserCommunityStatus Status { get; private set; }
    }
}
