using System.ComponentModel.DataAnnotations;
using Application.DTOs.Community;

namespace WebApi.Responses.Community
{
    /// <summary>
    /// Community creation response.
    /// </summary>
    public class CommunityDefaultResponse
    {
        /// <summary>
        /// Created Community
        /// </summary>
        [Required] public required CommunityGetDto Community { get; set; }

        /// <summary>
        /// Community creation message
        /// </summary>
        [Required] public required string Message { get; set; }

        /// <summary>
        /// Community creation status
        /// </summary>
        [Required] public required int Status { get; set; }
    }
}
