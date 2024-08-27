using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class UserLoginPostDto
    {
        [Required][MaxLength(80)] public required string Username { get; set; }
        [Required][PasswordPropertyText] public required string Password { get; set; }
    }
}
