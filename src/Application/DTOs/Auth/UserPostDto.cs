using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Application.DTOs.Auth
{
    public class UserPostDto
    {
        [Required][MaxLength(80)] public required string Username { get; set; }
        [Required][EmailAddress] public required string Email { get; set; }
        [Required][PasswordPropertyText] public required string Password { get; set; }
        [Required][PasswordPropertyText] public required string ConfirmPassword { get; set; }
    }
}
