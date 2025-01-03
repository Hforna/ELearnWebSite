using AutoMapper;
using Microsoft.Identity.Client;
using Sqids;
using User.Api.DTOs;
using User.Api.Models;

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
        }
    }
}
