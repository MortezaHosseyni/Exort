using System.ComponentModel.DataAnnotations;
using Application.DTOs.User;

namespace WebApi.Responses
{
    /// <summary>
    /// Return information about login (token)
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Authenticated User information.
        /// </summary>
        [Required] public required UserOfficialGetDto User { get; set; }

        /// <summary>
        /// Authentication message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Authenticated User token
        /// </summary>
        [Required] public required string Token { get; set; }
    }
}
