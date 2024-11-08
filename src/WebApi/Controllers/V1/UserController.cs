﻿using Application.DTOs.User;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Responses;
using WebApi.Responses.Auth;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represent User endpoints.
    /// </summary>
    /// <param name="user">User service</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController(IUserService user) : ControllerBase
    {
        private readonly IUserService _user = user;

        /// <summary>
        /// Update authenticated User information.
        /// </summary>
        /// <param name="user">User Information</param>
        /// <returns>User Model</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(typeof(UserOfficialGetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put([FromForm] UserPutDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "User information is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var updatedResult = await _user.UpdateUser(Ulid.Parse(currentUser.Value), user);

                if (updatedResult.Item2)
                    return Ok(new UserUpdateResponse() { User = updatedResult.Item1!, Message = updatedResult.Item3, Status = 200 });
                else
                    return BadRequest(new DefaultResponse() { Message = updatedResult.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Update authenticated User password.
        /// </summary>
        /// <param name="updatePassword">User Password Information</param>
        /// <returns>User Model</returns>
        [HttpPut("UpdatePassword")]
        [Authorize]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePassword([FromBody] UserUpdatePasswordDto updatePassword)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Password model is invalid.", Status = 400 });

                var currentUser = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUser == null)
                    return Unauthorized(new DefaultResponse() { Message = "User is not authenticated.", Status = 401 });

                var updatedPassword = await _user.UpdatePassword(Ulid.Parse(currentUser.Value), updatePassword);

                if (updatedPassword.Item1)
                    return Ok(new DefaultResponse() { Message = updatedPassword.Item2, Status = 200 });
                else
                    return BadRequest(new DefaultResponse() { Message = updatedPassword.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
