using Application.DTOs.Community;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Responses;
using WebApi.Responses.Community;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represent Community endpoints.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommunityController(ICommunityService community) : ControllerBase
    {
        private readonly ICommunityService _community = community;

        /// <summary>
        /// Get authenticated User all joined Communities.
        /// </summary>
        /// <returns>List of Community Model</returns>
        [HttpGet("MyCommunities")]
        [ProducesResponseType(typeof(List<CommunityGetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMyCommunities()
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var myCommunities = await _community.GetUserCommunities(Ulid.Parse(currentUser.Value));
                if (!myCommunities.Any())
                    return NotFound(new DefaultResponse()
                    { Message = "There is no Community this User joined in this.", Status = 404 });

                return Ok(myCommunities);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Create a new Community.
        /// </summary>
        /// <param name="community">Community information</param>
        /// <returns>Created Community Model</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CommunityDefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Post([FromForm] CommunityPostDto community)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var createCommunity = await _community.AddCommunity(community, Ulid.Parse(currentUser.Value));

                return createCommunity.Item2 ?
                    StatusCode(StatusCodes.Status201Created, new CommunityDefaultResponse() { Community = createCommunity.Item1!, Message = createCommunity.Item3, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = createCommunity.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Update Community information.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <param name="community">Community Information</param>
        /// <returns>Updated Community Model</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(CommunityDefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Put(Ulid id, [FromForm] CommunityPutDto community)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var updateCommunity = await _community.UpdateCommunity(id, Ulid.Parse(currentUser.Value), community);

                return updateCommunity.Item2 ?
                    Ok(new CommunityDefaultResponse() { Community = updateCommunity.Item1!, Message = updateCommunity.Item3, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = updateCommunity.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Join to a Community.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <returns>Join Status</returns>
        [HttpPatch("Join/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> JoinCommunity(Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var joinToCommunity = await _community.JoinToCommunity(Ulid.Parse(currentUser.Value), id);

                return joinToCommunity.Item1 ?
                    Ok(new DefaultResponse() { Message = joinToCommunity.Item2, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = joinToCommunity.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Leave from a Community.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <returns>Leave Community Status</returns>
        [HttpDelete("Leave/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> LeaveCommunity(Ulid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var leaveTheCommunity = await _community.LeaveTheCommunity(Ulid.Parse(currentUser.Value), id);

                return leaveTheCommunity.Item1 ?
                    Ok(new DefaultResponse() { Message = leaveTheCommunity.Item2, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = leaveTheCommunity.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
