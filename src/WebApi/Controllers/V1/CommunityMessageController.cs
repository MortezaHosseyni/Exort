using Application.DTOs.CommunityMessage;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Responses;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represents Community Messages endpoints.
    /// </summary>
    /// <param name="message">Community Message service</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommunityMessageController(ICommunityMessageService message) : ControllerBase
    {
        private readonly ICommunityMessageService _message = message;

        /// <summary>
        /// Get Community text Channel Messages.
        /// </summary>
        /// <param name="id">Channel ID</param>
        /// <param name="beforeTimestamp">Last Message timestamp</param>
        /// <returns>List of Community Messages Model</returns>
        [HttpGet("ChannelMessages/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<CommunityMessageGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCommunityChannelMessages([FromRoute] Ulid id, [FromQuery] DateTime? beforeTimestamp)
        {
            try
            {
                var messages = await _message.GetChannelMessages(id, beforeTimestamp);

                if (!messages.Any())
                    return NotFound(new DefaultResponse()
                        { Message = "There is no Messages in this Channel of Community.", Status = 404 });

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Send a Message to a Channel of a Community.
        /// </summary>
        /// <param name="message">Message Information</param>
        /// <returns>Message sending status</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Post([FromForm] CommunityMessagePostDto message)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community Message model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var addMessage = await _message.AddMessage(Ulid.Parse(currentUser.Value), message);

                return addMessage.Item1 ?
                    StatusCode(StatusCodes.Status201Created, new DefaultResponse() { Message = addMessage.Item2, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = addMessage.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Remove a Message from a Community Channel.
        /// </summary>
        /// <param name="id">Message ID</param>
        /// <returns>Message deletion status</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Delete([FromRoute] Ulid id)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUser == null)
                return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

            var removeMessage = await _message.RemoveMessage(Ulid.Parse(currentUser.Value), id);

            return removeMessage.Item1 ?
                StatusCode(StatusCodes.Status204NoContent, new DefaultResponse() { Message = removeMessage.Item2, Status = 204 }) :
                BadRequest(new DefaultResponse() { Message = removeMessage.Item2, Status = 400 });
        }
    }
}
