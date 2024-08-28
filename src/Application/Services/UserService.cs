using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.User;
using Shared.Utilities;

namespace Application.Services
{
    public interface IUserService
    {
        Task<UserGetDto> GetUser(Ulid id);
        Task<(UserOfficialGetDto?, bool, string)> UpdateUser(Ulid id, UserPutDto user);
        Task<(bool, string)> UpdatePassword(Ulid id, UserUpdatePasswordDto updatePassword);
    }
    public class UserService(IUserRepository user, IFileManagerService fileManager, IMapper mapper) : IUserService
    {
        private readonly IUserRepository _user = user;

        private readonly IFileManagerService _fileManager = fileManager;
        private readonly IMapper _mapper = mapper;

        public async Task<UserGetDto> GetUser(Ulid id)
        {
            var filter = Builders<User>.Filter.Gt(u => u.Id, id);
            var user = await _user.FindOneAsync(filter);

            return _mapper.Map<UserGetDto>(user);
        }

        public async Task<(UserOfficialGetDto?, bool, string)> UpdateUser(Ulid id, UserPutDto user)
        {
            try
            {
                // Find User
                var filter = Builders<User>.Filter.Gt(u => u.Id, id);
                var userModel = await _user.FindOneAsync(filter);

                // Update avatar
                var userAvatar = userModel.Avatar;
                if (user.Avatar != null)
                    userAvatar = _fileManager.SaveFileAndReturnName(user.Avatar, $"wwwroot/Users/{userModel.Id}/Avatars");

                // Update User
                var updatedUser = new User(user.FirstName, user.LastName, null, userModel.Username, user.Bio, null, null, userAvatar, user.SocialMedias, UserGender.Undefined,
                    UserStatus.Active, userModel.StatusDescription, userModel.RegisterIp, null)
                {
                    Id = userModel.Id,
                    PhoneNumberConfirmation = false,
                    Email = userModel.Email,
                    EmailConfirmation = false,
                    Password = userModel.Password,
                    UserAgent = userModel.UserAgent,
                    CreateDateTime = userModel.CreateDateTime,
                    UpdateDateTime = DateTime.Now
                };

                await _user.UpdateAsync(filter, updatedUser);

                return (_mapper.Map<UserOfficialGetDto>(updatedUser), true, "User updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
        }

        public async Task<(bool, string)> UpdatePassword(Ulid id, UserUpdatePasswordDto updatePassword)
        {
            try
            {
                // Confirm password
                if (updatePassword.Password != updatePassword.ConfirmPassword)
                    return (false, "Password and confirm password is not match.");

                // Find and check User
                var userFilter = Builders<User>.Filter.Gt(u => u.Id, id);
                if (!await _user.AnyByPredicateAsync(userFilter))
                    return (false, "Couldn't find any User with that Id.");

                var user = await _user.FindOneAsync(userFilter);

                // Update password
                user.Password = Password.Hash(updatePassword.Password);
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
