using Application.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Responses.Auth
{
    /// <summary>
    /// User update response
    /// </summary>
    public class UserUpdateResponse
    {
        /// <summary>
        /// Updated User information.
        /// </summary>
        [Required] public required UserOfficialGetDto User { get; set; }

        /// <summary>
        /// Update message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Update Status.
        /// </summary>
        [Required] public required int Status { get; set; }
    }
}
