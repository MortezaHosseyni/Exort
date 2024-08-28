using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Formats;
using System.Text.Json;
using WebApi.Responses;

namespace WebApi.Controllers.V1
{
    /// <summary>
    /// Represent authentication endpoints and services.
    /// </summary>
    /// <param name="auth">Authentication service.</param>
    /// <param name="configuration">Configuration service.</param>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController(IAuthService auth, IConfiguration configuration) : ControllerBase
    {
        private readonly IAuthService _auth = auth;

        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Authenticate user and return token.
        /// </summary>
        /// <returns>JWT (Token)</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody] UserLoginPostDto login)
        {
            try
            {
                if (ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Login information is invalid.", Status = 400 });

                var jwtSettings = JsonSerializer.Deserialize<JwtInformationFormat>(_configuration["Jwt"]!);
                if (jwtSettings == null)
                    return BadRequest(new DefaultResponse() { Message = "Error occur in authentication system!", Status = 400 });

                var loginResult = await _auth.Login(login, jwtSettings);

                if (loginResult.Item2)
                {
                    return Ok(new LoginResponse()
                    {
                        Message = "User authenticated successfully.",
                        Token = loginResult.Item3,
                        User = loginResult.Item1!
                    });
                }
                else
                {
                    return BadRequest(new DefaultResponse() { Message = loginResult.Item3, Status = 400 });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
