using Domain.Entities;
using Shared.Enums.User;

namespace Application.DTOs.User
{
    public class UserOfficialGetDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Bio { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public Dictionary<string, string>? SocialMedias { get; set; }
        public UserGender Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime LastLogin { get; set; }
        public ICollection<Friend>? Friends { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
    }
}
