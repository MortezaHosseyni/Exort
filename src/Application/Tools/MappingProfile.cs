using Application.DTOs.User;
using AutoMapper;
using Domain.Entities;

namespace Application.Tools
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserGetDto>();
        }
    }
}
