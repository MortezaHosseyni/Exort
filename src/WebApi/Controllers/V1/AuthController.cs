using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Formats;
using System.Text.Json;
using WebApi.Responses;
using WebApi.Responses.Auth;

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
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Login information is invalid.", Status = 400 });

                var jwtSettings = JsonSerializer.Deserialize<JwtInformationFormat>(_configuration["Jwt"]!);
                if (jwtSettings == null)
                    return BadRequest(new DefaultResponse() { Message = "Error occur in authentication system!", Status = 400 });

                var loginResult = await _auth.Login(login, jwtSettings);

                if (loginResult.Item2)
                    return Ok(new LoginResponse()
                    {
                        Message = "User authenticated successfully.",
                        Token = loginResult.Item3,
                        User = loginResult.Item1!
                    });
                else
                    return BadRequest(new DefaultResponse() { Message = loginResult.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Register a new User.
        /// </summary>
        /// <param name="user">User information</param>
        /// <returns>Registered User information</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Register([FromBody] UserPostDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Register information is invalid.", Status = 400 });

                var registerResult =
                    await _auth.Register(user, Request.Headers["X-Real-IP"], Request.Headers["User-Agent"]);

                if (registerResult.Item2)
                    return StatusCode(StatusCodes.Status201Created,
                        new RegisterResponse() { RegisteredUser = registerResult.Item1!, Message = registerResult.Item3, Status = 201 });
                else
                    return BadRequest(new DefaultResponse() { Message = registerResult.Item3, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Send reset password information to User.
        /// </summary>
        /// <param name="forgotPassword">User Information</param>
        /// <returns>Status and Message</returns>
        [HttpPost("ForgotPassword")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ForgotPassword([FromBody] UserForgotPasswordDto forgotPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Forgot password information is invalid.", Status = 400 });

                var forgotPasswordResult = await _auth.ForgotPassword(forgotPassword);

                if (forgotPasswordResult.Item1)
                    return Ok(new DefaultResponse() { Message = forgotPasswordResult.Item2, Status = 200 });
                else
                    return BadRequest(new DefaultResponse() { Message = forgotPasswordResult.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }

        /// <summary>
        /// Reset forgot password.
        /// </summary>
        /// <param name="resetPassword">Reset Password Information</param>
        /// <returns>Reset password status.</returns>
        [HttpPost("ResetForgotPassword")]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetForgotPassword([FromBody] UserResetForgotPasswordDto resetPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new DefaultResponse() { Message = "Reset password information is invalid.", Status = 400 });

                var resetPasswordResult = await _auth.ResetForgotPassword(resetPassword);

                if (resetPasswordResult.Item1)
                    return Ok(new DefaultResponse() { Message = resetPasswordResult.Item2, Status = 200 });
                else
                    return BadRequest(new DefaultResponse() { Message = resetPasswordResult.Item2, Status = 400 });
            }
            catch (Exception ex)
            {
                return BadRequest(new DefaultResponse() { Message = ex.Message, Status = 400 });
            }
        }
    }
}
