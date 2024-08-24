using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Application.DTOs.User
{
    public class UserUpdatePasswordDto
    {
        [Required][PasswordPropertyText] public required string OldPassword { get; set; }
        [Required][PasswordPropertyText] public required string Password { get; set; }
        [Required][PasswordPropertyText] public required string ConfirmPassword { get; set; }
    }
}