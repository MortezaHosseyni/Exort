using Microsoft.AspNetCore.Http;
using Shared.Enums.Community;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Community
{
    public class CommunityPutDto
    {
        [Required][MaxLength(80)] public required string Name { get; set; }
        [Required][MaxLength(300)] public required string Description { get; set; }
        [MaxLength(500)] public IFormFile? Image { get; set; }
        [MaxLength(500)] public IFormFile? Banner { get; set; }
        public CommunityType Type { get; set; }
    }
}
