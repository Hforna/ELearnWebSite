using AutoMapper;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class GetLessonShortInfos : IGetLessonShortInfos
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public GetLessonShortInfos(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        public async Task<LessonShortInfosResponse> Execute(long lessonId)
        {
            var lesson = await _uof.lessonRead.LessonById(lessonId);

            if (lesson is null)
                throw new LessonException(ResourceExceptMessages.LESSON_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var response = _mapper.Map<LessonShortInfosResponse>(lesson);

            return response;
        }
    }
}
