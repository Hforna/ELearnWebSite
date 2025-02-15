using AutoMapper;
using Azure;
using Microsoft.Identity.Client;
using Sqids;
using User.Api.DTOs;
using User.Api.Models;
using User.Api.Responses;

namespace User.Api.Services.AutoMapper
{
    public class MapperService : Profile
    {
        private readonly SqidsEncoder<long> _sqids;

        public MapperService(SqidsEncoder<long> sqids)
        {
            _sqids = sqids;

            CreateMap<CreateUserDto, UserModel>()
                .ForMember(d => d.PasswordHash, f => f.Ignore());

            CreateMap<UserModel, UserResponse>()
                .ForMember(d => d.Id, f => f.MapFrom(d => _sqids.Encode(d.Id)));

            CreateMap<CreateProfileDto, ProfileModel>()
                .ForMember(d => d.ProfilePicture, d => d.Ignore());

            CreateMap<Profile, ProfileShortResponse>();

            CreateMap<ProfileModel, ResponseProfile>()
                .ForMember(response => response.UserId, d => d.MapFrom(d => _sqids.Encode(d.UserId)))
                .ForMember(response => response.Id, d => d.MapFrom(d => _sqids.Encode(d.Id)));
        }
    }
}
