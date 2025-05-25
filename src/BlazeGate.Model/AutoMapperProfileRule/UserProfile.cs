using AutoMapper;
using BlazeGate.Model.EFCore;
using BlazeGate.Model.JwtBearer;
using BlazeGate.Model.WebApi.Request;
using BlazeGate.Model.WebApi.Response;

namespace BlazeGate.Model.AutoMapperProfileRule
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<UserInfo, User>();
            CreateMap<User, UserInfo>();

            CreateMap<UserSave, User>();
            CreateMap<User, UserSave>();
        }
    }
}