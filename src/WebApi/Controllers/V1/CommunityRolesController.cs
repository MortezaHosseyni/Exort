using Application.DTOs.Community;
using Application.DTOs.CommunityPart;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Responses;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represents Community Roles endpoints.
    /// </summary>
    /// <param name="roles">Roles service</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommunityRolesController(ICommunityRolesService roles) : ControllerBase
    {
        private readonly ICommunityRolesService _roles = roles;

        /// <summary>
        /// Get a Community Roles.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <returns>List Of Community Roles</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Dictionary<string, List<CommunityPartGetDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCommunityRoles([FromRoute] Ulid id)
        {
            try
            {
                var roles = await _roles.GetCommunityRoles(id);
                if (!roles.Any())
                    return NotFound(new DefaultResponse() { Message = "There is not Role registered for this Community.", Status = 404 });

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Add a new Role for Community.
        /// </summary>
        /// <param name="role">Role Information</param>
        /// <returns>Status of Role creation</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Post([FromBody] CommunityRolePostDto role)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community Role model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var addRole = await _roles.AddRoleToCommunity(Ulid.Parse(currentUser.Value), role);

                return addRole.Item1 ?
                    StatusCode(StatusCodes.Status201Created, new DefaultResponse() { Message = addRole.Item2, Status = 201 }) :
                    BadRequest(new DefaultResponse() { Message = addRole.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Remove a Role from Community.
        /// </summary>
        /// <param name="id">Community ID</param>
        /// <param name="name">Role Name</param>
        /// <returns>Status of Role deletion</returns>
        [HttpDelete("{id}/{name}")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Delete([FromRoute] Ulid id, [FromRoute] string name)
        {
            try
            {
                if (id == Ulid.Empty || string.IsNullOrEmpty(name))
                    return BadRequest(new DefaultResponse() { Message = "Community Id or Role name is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var removeRole = await _roles.RemoveRoleFromCommunity(name, Ulid.Parse(currentUser.Value), id);

                return removeRole.Item1 ?
                    StatusCode(StatusCodes.Status204NoContent, new DefaultResponse() { Message = removeRole.Item2, Status = 204 }) :
                    BadRequest(new DefaultResponse() { Message = removeRole.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Assign a Role to a Community member.
        /// </summary>
        /// <param name="role">Role Information</param>
        /// <returns>Status of role assigning</returns>
        [HttpPatch("Assign")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AssignRoleToCommunityMember([FromBody] CommunityRolePatchDto role)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community Role model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var giveRoleToMember = await _roles.GiveRoleToUser(role.RoleName, Ulid.Parse(currentUser.Value),
                    role.CommunityId, role.MemberId);

                return giveRoleToMember.Item1 ?
                    Ok(new DefaultResponse() { Message = giveRoleToMember.Item2, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = giveRoleToMember.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Depose a member Role in Community.
        /// </summary>
        /// <param name="role">Role Information</param>
        /// <returns>Status of role deposing</returns>
        [HttpPatch("Depose")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeposeRoleFromCommunityMember([FromBody] CommunityRolePatchDto role)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Community Role model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var deposeRoleFromMember = await _roles.RemoveUserRole(role.RoleName, Ulid.Parse(currentUser.Value),
                    role.CommunityId, role.MemberId);

                return deposeRoleFromMember.Item1 ?
                    Ok(new DefaultResponse() { Message = deposeRoleFromMember.Item2, Status = 200 }) :
                    BadRequest(new DefaultResponse() { Message = deposeRoleFromMember.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
