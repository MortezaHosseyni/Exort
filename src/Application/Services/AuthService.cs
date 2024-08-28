using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs.Auth;
using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.User;
using Shared.Utilities;
using System.Text;
using Infrastructure.Adapters.Email;
using Microsoft.IdentityModel.Tokens;
using Shared.Formats;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<(UserOfficialGetDto?, bool, string)> Login(UserLoginPostDto login, JwtInformationFormat jwt);
        Task<(UserOfficialGetDto?, bool, string)> Register(UserPostDto user, string? ip, string? agent);
        Task<(bool, string)> ForgotPassword(UserForgotPasswordDto password);
        Task<(bool, string)> ResetForgotPassword(UserResetForgotPasswordDto resetPassword);
    }
    public class AuthService(IUserRepository user, IEmailService email, IMapper mapper) : IAuthService
    {
        private readonly IUserRepository _user = user;
        private readonly IEmailService _email = email;

        private readonly IMapper _mapper = mapper;

        public async Task<(UserOfficialGetDto?, bool, string)> Login(UserLoginPostDto login, JwtInformationFormat jwt)
        {
            try
            {
                // Find and check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Username, login.Username);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (null, false, "Couldn't find any User with that username.");

                var user = await _user.FindOneAsync(userFilter);

                // Check password
                if (Password.VerifyPassword(login.Password, user.Password))
                {
                    // Declare credentials
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    // Declare claims
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Exp, Math.Floor((decimal)(DateTime.UtcNow.AddDays(1)).Ticks / 10000000).ToString(CultureInfo.InvariantCulture))
                    };

                    // Declare token
                    var token = new JwtSecurityToken(
                        jwt.Issuer,
                        jwt.Audience,
                        claims,
                        expires: DateTime.Now.AddMinutes(Convert.ToInt32(jwt.ExpirationInMinutes)),
                        signingCredentials: credentials
                    );

                    // Generate token
                    var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
                    var loggedInUser = _mapper.Map<UserOfficialGetDto>(user);

                    return (loggedInUser, true, generatedToken);
                }
                else
                {
                    return (null, false, "Username or password is incorrect.");
                }
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(UserOfficialGetDto?, bool, string)> Register(UserPostDto user, string? ip, string? agent)
        {
            try
            {
                // Check unique fields
                var userFilter = Builders<User>.Filter.Gt(u => u.Username, user.Username) | Builders<User>.Filter.Gt(u => u.Email, user.Email);
                if (await _user.AnyByPredicateAsync(userFilter))
                    return (null, false, "This email or username already used.");

                // Confirm password
                if (user.Password != user.ConfirmPassword)
                    return (null, false, "Password and confirm password is not match.");

                // Add User
                var addedUser = new User(null, null, null, user.Username, null, null, null, null, null, UserGender.Undefined,
                    UserStatus.Active, "User registered and active.", ip, null)
                {
                    PhoneNumberConfirmation = false,
                    Email = user.Email,
                    EmailConfirmation = false,
                    Password = Password.Hash(user.Password),
                    UserAgent = agent,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };

                await _user.AddAsync(addedUser);

                return (_mapper.Map<UserOfficialGetDto>(addedUser), true, "User registered successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(bool, string)> ForgotPassword(UserForgotPasswordDto password)
        {
            try
            {
                // Find and check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Email, password.Email);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that email.");

                var user = await _user.FindOneAsync(userFilter);

                // Send Code
                var code = CodeGenerator.GenerateNumberCode(6);
                await _email.SendEmailAsync(password.Email, "Exort | Forgot Password", $"Your reset password code is: {code}", false); // TODO: Create beautiful html page for email body.

                user.ResetPasswordCode = code;
                user.ResetPasswordExpireTime = DateTime.Now.AddMinutes(30);
                await _user.UpdateAsync(userFilter, user);

                return (true, "Reset password code sent to email successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool, string)> ResetForgotPassword(UserResetForgotPasswordDto resetPassword)
        {
            try
            {
                // Find and check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Email, resetPassword.Email);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that email.");

                var user = await _user.FindOneAsync(userFilter);

                // Check code
                if (resetPassword.Code != user.ResetPasswordCode)
                    return (false, "The reset password code is invalid.");

                // Check expiration
                if (DateTime.Now > user.ResetPasswordExpireTime)
                    return (false, "Reset password time is expired!");

                // Confirm password
                if (resetPassword.Password != resetPassword.ConfirmPassword)
                    return (false, "Password and confirm password is not match.");

                // Update password
                user.ResetPasswordCode = string.Empty;
                user.ResetPasswordExpireTime = DateTime.MinValue;
                user.Password = Password.Hash(resetPassword.Password);
                await _user.UpdateAsync(userFilter, user);

                return (true, "User password updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
