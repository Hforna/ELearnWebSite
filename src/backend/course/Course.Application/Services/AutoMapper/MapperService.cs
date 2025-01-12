using AutoMapper;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services.AutoMapper
{
    public class MapperService : Profile
    {
        public MapperService()
        {
            RequestToEntity();
            EntityToResponse();
        }

        private void RequestToEntity()
        {
            CreateMap<CreateCourseRequest, CourseEntity>()
                .ForMember(d => d.Thumbnail, f => f.Ignore());

            CreateMap<CreateCourseTopicsRequest, CourseTopicsEntity>();
        }

        private void EntityToResponse()
        {
            CreateMap<CourseEntity, CourseResponse>();
            CreateMap<CourseTopicsEntity, CourseTopicsResponse>();
        }
    }
}
