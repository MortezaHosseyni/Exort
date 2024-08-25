using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Shared.Enums.User;

namespace Application.Services
{
    public interface IUserService
    {
        Task<UserGetDto> GetUser(Ulid id);
        Task<(UserGetDto?, string)> AddUser(UserPostDto user, string ip, string agent);
        Task<(UserGetDto?, string)> UpdateUser(Ulid id, UserPutDto user);
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

        public async Task<(UserGetDto?, string)> AddUser(UserPostDto user, string ip, string agent)
        {
            try
            {
                var userModel = new User(null, null, null, user.Username, null, null, null, null, null, UserGender.Undefined,
                    UserStatus.Active, "User registered and active.", ip, null)
                {
                    PhoneNumberConfirmation = false,
                    Email = user.Email,
                    EmailConfirmation = false,
                    Password = user.Password,
                    UserAgent = agent,
                    CreateDateTime = DateTime.Now,
                    UpdateDateTime = DateTime.Now
                };

                await _user.AddAsync(userModel);

                return (_mapper.Map<UserGetDto>(userModel), "User registered successfully.");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(UserGetDto?, string)> UpdateUser(Ulid id, UserPutDto user)
        {
            try
            {
                var filter = Builders<User>.Filter.Gt(u => u.Id, id);
                var userModel = await _user.FindOneAsync(filter);

                var userAvatar = userModel.Avatar;
                if (user.Avatar != null)
                    userAvatar = _fileManager.SaveFileAndReturnName(user.Avatar, $"wwwroot/Users/{userModel.Id}/Avatars");

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

                return (_mapper.Map<UserGetDto>(updatedUser), "User updated successfully.");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }
    }
}
