using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Application.DTOs.Auth
{
    public class UserForgotPasswordDto
    {
        [Required][PasswordPropertyText] public required string RememberPassword { get; set; }
        [Required][EmailAddress] public required string Email { get; set; }
    }
}
