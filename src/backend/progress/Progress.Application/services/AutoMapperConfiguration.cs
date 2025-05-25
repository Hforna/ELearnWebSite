using AutoMapper;
using Progress.Application.Responses;
using Progress.Domain.Dtos;
using Progress.Domain.Entities;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.services
{
    public class AutoMapperConfiguration : Profile
    {
        private readonly SqidsEncoder<long> _sqids;

        public AutoMapperConfiguration(SqidsEncoder<long> sqids) => _sqids = sqids;

        public AutoMapperConfiguration()
        {
            CreateMap<QuizDto, ShortQuizAnswersResponse>()
                .ForMember(d => d.PassingScore, f => f.MapFrom(d => d.passingScore))
                .ForMember(d => d.QuizId, f => f.MapFrom(d => d.id));

            CreateMap<QuestionResponse, QuestionAnswerDto>()
                .ForMember(d => d.QuestionId, f => f.MapFrom(d => d.id))
                .ForMember(d => d.AnswerId, f => f.Condition(d => d.AnswerOptions.Where(d => d.isCorrect).Any()));

            CreateMap<AnswerOptionsResponse, QuestionAnswerResponse>()
                .ForMember(d => d.isCorrect, f => f.MapFrom(d => d.isCorrect))
                .ForMember(d => d.QuestionId, f => f.MapFrom(d => d.questionId))
                .ForMember(d => d.Id, f => f.MapFrom(d => d.id));

            CreateMap<QuestionResponse, FullQuestionResponse>()
                .ForMember(d => d.QuestionId, f => f.MapFrom(d => d.id))
                .ForMember(d => d.Answers, f => f.MapFrom(d => d.AnswerOptions));

            CreateMap<UserCourseProgress, UserCourseProgressResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(d => _sqids.Encode(d.CourseId)))
                .ForMember(d => d.UserId, f => f.MapFrom(d => _sqids.Encode(d.UserId)));
        }
    }
}
