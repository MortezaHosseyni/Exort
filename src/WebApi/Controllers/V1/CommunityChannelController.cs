using System.Security.Claims;
using Application.DTOs.CommunityChannel;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using WebApi.Responses;
using WebApi.Responses.Community;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represent Community Channel endpoints.
    /// </summary>
    /// <param name="channel">Community Channel Service</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommunityChannelController(ICommunityChannelService channel) : ControllerBase
    {
        private readonly ICommunityChannelService _channel = channel;

        /// <summary>
        /// Get channel of a Community.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <returns>List of Community Channel Model</returns>
        [HttpGet("CommunityChannels")]
        [ProducesResponseType(typeof(List<CommunityChannelGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCommunityChannels(Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var channels = await _channel.GetCommunityChannels(id, Ulid.Parse(currentUser.Value));

                if (channels.Item1 == null)
                    return BadRequest(new DefaultResponse()
                        { Message = channels.Item2, Status = 404 });

                if (channels.Item1 != null && !channels.Item1.Any())
                    return NotFound(new DefaultResponse()
                        { Message = "There is no Community Channel created in this Community.", Status = 404 });

                return Ok(channels.Item1);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Create a new Channel for a Community.
        /// </summary>
        /// <param name="channel">Channel information</param>
        /// <returns>Created Channel status</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CommunityChannelDefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Post([FromBody] CommunityChannelPostDto channel)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var createChannel = await _channel.AddCommunityChannel(Ulid.Parse(currentUser.Value), channel);

                return createChannel.Item2 ?
                    StatusCode(StatusCodes.Status201Created, new CommunityChannelDefaultResponse() { CommunityChannel = createChannel.Item1!, Message = createChannel.Item3, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = createChannel.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Update a Community Channel.
        /// </summary>
        /// <param name="id">Community Channel ID</param>
        /// <param name="channel">Community Channel Information</param>
        /// <returns>Updated Channel status</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CommunityChannelDefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put([FromRoute] Ulid id, [FromForm] CommunityChannelPutDto channel)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var updateChannel = await _channel.UpdateCommunityChannel(Ulid.Parse(currentUser.Value), id, channel);

                return updateChannel.Item2 ?
                    Ok(new CommunityChannelDefaultResponse() { CommunityChannel = updateChannel.Item1!, Message = updateChannel.Item3, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = updateChannel.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Delete a Community Channel.
        /// </summary>
        /// <param name="id">Community Channel ID</param>
        /// <returns>Channel Deletion status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var deleteChannel = await _channel.DeleteChannel(Ulid.Parse(currentUser.Value), id);

                return deleteChannel.Item1 ?
                    StatusCode(StatusCodes.Status204NoContent, new DefaultResponse() { Message = deleteChannel.Item2, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = deleteChannel.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
