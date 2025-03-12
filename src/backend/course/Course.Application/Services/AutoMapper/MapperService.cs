﻿using AutoMapper;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Sqids;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Application.Services.AutoMapper
{
    public class MapperService : Profile
    {
        private readonly SqidsEncoder<long> _sqids;

        public MapperService(SqidsEncoder<long> sqids)
        {
            _sqids = sqids;

            RequestToEntity();
            EntityToResponse();
        }

        private void RequestToEntity()
        {
            CreateMap<CreateCourseRequest, CourseEntity>()
                .ForMember(d => d.Thumbnail, f => f.Ignore())
                .ForMember(d => d.TopicsCovered, f => f.MapFrom(f => f.CourseTopics));

            CreateMap<GetCoursesRequest, GetCoursesFilterDto>();

            CreateMap<CreateCourseTopicsRequest, CourseTopicsEntity>();

            CreateMap<UpdateModuleRequest, Module>();

            CreateMap<CreateLessonRequest, Lesson>();

            CreateMap<UpdateCourseRequest, CourseEntity>()
                .ForMember(d => d.Thumbnail, f => f.Ignore());

            CreateMap<CreateReviewRequest, Review>();

            CreateMap<CreateModuleRequest, Module>();
        }

        private void EntityToResponse()
        {
            CreateMap<CourseEntity, CourseShortResponse>()
                    .ForMember(d => d.CourseId, f => f.MapFrom(d => _sqids.Encode(d.Id)))
                    .ForMember(d => d.TeacherId, f => f.MapFrom(d => _sqids.Encode(d.TeacherId)));

            CreateMap<Review, ReviewResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(d => _sqids.Encode(d.CourseId)))
                .ForMember(d => d.Id, f => f.MapFrom(d => _sqids.Encode(d.Id)))
                .ForMember(d => d.CustomerId, f => f.MapFrom(d => _sqids.Encode(d.CustomerId)));

            CreateMap<Enrollment, EnrollmentResponse>();

            CreateMap<WishList, WishListResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(s => _sqids.Encode(s.CourseId)))
                .ForMember(d => d.Id, f => f.MapFrom(s => _sqids.Encode(s.Id)))
                .ForMember(d => d.UserId, f => f.MapFrom(s => _sqids.Encode(s.UserId)));

            CreateMap<CourseEntity, CourseResponse>()
                .ForMember(d => d.Id, f => f.MapFrom(d => _sqids.Encode(d.Id)))
                .ForMember(d => d.TeacherId, f => f.MapFrom(d => _sqids.Encode(d.TeacherId)));

            CreateMap<CourseTopicsEntity, CourseTopicsResponse>();

            CreateMap<Lesson, LessonResponse>()
                .ForMember(d => d.Id, f => f.MapFrom(d => _sqids.Encode(d.Id)))
                .ForMember(d => d.ModuleId, f => f.MapFrom(d => _sqids.Encode(d.ModuleId)));

            CreateMap<IPagedList<CourseEntity>, CoursesPaginationResponse>();

            CreateMap<Module, ModuleResponse>()
                .ForMember(d => d.CourseId, d => d.MapFrom(d => _sqids.Encode(d.CourseId)))
                .ForMember(d => d.Id, d => d.MapFrom(d => _sqids.Encode(d.Id)));

            CreateMap<List<Module>, ModulesResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(m => _sqids.Encode(m.FirstOrDefault().CourseId)))
                .ForMember(d => d.Modules, f => f.MapFrom(m => m));

            CreateMap<Collection<Module>, ModulesResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(m => _sqids.Encode(m.FirstOrDefault().CourseId)))
                .ForMember(d => d.Modules, f => f.MapFrom(m => m));
        }
    }
}
