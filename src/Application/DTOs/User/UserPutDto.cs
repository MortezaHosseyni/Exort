using Shared.Enums.User;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.User
{
    public class UserPutDto
    {
        [MaxLength(70)] public string? FirstName { get; set; }
        [MaxLength(80)] public string? LastName { get; set; }
        [MaxLength(100)] public string? Bio { get; set; }
        public IFormFile? Avatar { get; set; }
        public Dictionary<string, string>? SocialMedias { get; set; }
        public UserGender Gender { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
