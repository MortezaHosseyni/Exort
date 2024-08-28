using System.ComponentModel.DataAnnotations;
using Application.DTOs.User;

namespace WebApi.Responses.Auth
{
    /// <summary>
    /// User register to application information 
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Registered user information.
        /// </summary>
        [Required] public required UserOfficialGetDto RegisteredUser { get; set; }
        /// <summary>
        /// Register message.
        /// </summary>
        [Required] public required string Message { get; set; }
        /// <summary>
        /// Register status.
        /// </summary>
        [Required] public required int Status { get; set; }
    }
}
