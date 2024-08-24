using Domain.Entities;

namespace Application.DTOs.User
{
    public class UserGetDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get;  set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public Dictionary<string, string>? SocialMedias { get; set; }
        public List<Friend>? MutualFriends { get; set; }
    }
}
