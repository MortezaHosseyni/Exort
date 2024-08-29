using Application.DTOs.Friend;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Responses;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represents Friend model endpoints.
    /// </summary>
    /// <param name="friend">Friend service</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FriendController(IFriendService friend) : ControllerBase
    {
        private readonly IFriendService _friend = friend;

        /// <summary>
        /// Get list of User Friends.
        /// </summary>
        /// <returns>List of Friend Model</returns>
        [HttpGet("MyFriends")]
        [Authorize]
        [ProducesResponseType(typeof(List<FriendGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMyFriends()
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var friends = await _friend.GetFriends(Ulid.Parse(currentUser.Value));
                if (!friends.Any())
                    return NotFound(new DefaultResponse()
                        { Message = "There is no Friend for this User.", Status = 404 });

                return Ok(friends);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Get mutual Friends between User and Friend.
        /// </summary>
        /// <param name="id">Friend ID</param>
        /// <returns>List of Friend model</returns>
        [HttpGet("MutualFriends/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<FriendGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMutualFriends([FromRoute] Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var friends = await _friend.GetMutualFriends(Ulid.Parse(currentUser.Value), id);
                if (!friends.Any())
                    return NotFound(new DefaultResponse()
                        { Message = "There is no Mutual Friend between these Users.", Status = 404 });

                return Ok(friends);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Add Friend for a User (Send invite).
        /// </summary>
        /// <param name="id">Friend ID</param>
        /// <returns>Friend adding status</returns>
        [HttpPost("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Post([FromRoute] Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var addFriend = await _friend.AddFriend(Ulid.Parse(currentUser.Value), id);

                return addFriend.Item1 ?
                    StatusCode(StatusCodes.Status201Created, new DefaultResponse() { Message = addFriend.Item2, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = addFriend.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Accept add Friend request.
        /// </summary>
        /// <param name="id">Friendship Model Id</param>
        /// <returns>Friend accepting status</returns>
        [HttpPatch("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AcceptInvite([FromRoute] Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var acceptInvite = await _friend.AcceptFriend(Ulid.Parse(currentUser.Value), id);

                return acceptInvite.Item1 ?
                    Ok(new DefaultResponse() { Message = acceptInvite.Item2, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = acceptInvite.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Remove a Friend for User.
        /// </summary>
        /// <param name="id">Friendship model Id</param>
        /// <param name="friendId">Friend Id</param>
        /// <returns>Friend deletion status</returns>
        [HttpDelete("{id}/{friendId}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Delete([FromRoute] Ulid id, [FromRoute] Ulid friendId)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var removeFriend = await _friend.RemoveFriend(id, Ulid.Parse(currentUser.Value), friendId);

                return removeFriend.Item1 ?
                    StatusCode(StatusCodes.Status204NoContent, new DefaultResponse() { Message = removeFriend.Item2, Status = 204 }) :
                    BadRequest(new DefaultResponse() { Message = removeFriend.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
