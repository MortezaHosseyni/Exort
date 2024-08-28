using System.ComponentModel.DataAnnotations;

namespace WebApi.Responses
{
    /// <summary>
    /// Return message and status code.
    /// </summary>
    public class DefaultResponse
    {
        /// <summary>
        /// Response message
        /// </summary>
        [Required] public required string Message { get; set; }

        /// <summary>
        /// Response status code
        /// </summary>
        [Required] public required int Status { get; set; }
    }
}
