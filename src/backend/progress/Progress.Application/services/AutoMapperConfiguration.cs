using AutoMapper;
using Progress.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.services
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<QuizDto, ShortQuizAnswersResponse>()
                .ForMember(d => d.PassingScore, f => f.MapFrom(d => d.passingScore))
                .ForMember(d => d.QuizId, f => f.MapFrom(d => d.id));

            CreateMap<QuestionResponse, QuestionAnswerDto>()
                .ForMember(d => d.QuestionId, f => f.MapFrom(d => d.id))
                .ForMember(d => d.AnswerId, f => f.Condition(d => d.AnswerOptions.Where(d => d.isCorrect).Any()));
        }
    }
}
