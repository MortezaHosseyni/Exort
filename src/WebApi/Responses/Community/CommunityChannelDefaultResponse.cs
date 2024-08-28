using System.ComponentModel.DataAnnotations;
using Application.DTOs.CommunityChannel;

namespace WebApi.Responses.Community
{
    /// <summary>
    /// Return Community Channel status
    /// </summary>
    public class CommunityChannelDefaultResponse
    {
        /// <summary>
        /// Community Channel model
        /// </summary>
        [Required] public required CommunityChannelGetDto CommunityChannel { get; set; }

        /// <summary>
        /// Community Channel message
        /// </summary>
        [Required] public required string Message { get; set; }

        /// <summary>
        /// Community Channel status
        /// </summary>
        [Required] public required int Status { get; set; }
    }
}
