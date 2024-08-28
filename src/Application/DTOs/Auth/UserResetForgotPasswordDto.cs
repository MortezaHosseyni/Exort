using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class UserResetForgotPasswordDto
    {
        [Required][EmailAddress] public required string Email { get; set; }
        [Required][MaxLength(6)] public required string Code { get; set; }
        [Required][PasswordPropertyText] public required string Password { get; set; }
        [Required][PasswordPropertyText] public required string ConfirmPassword { get; set; }
    }
}
