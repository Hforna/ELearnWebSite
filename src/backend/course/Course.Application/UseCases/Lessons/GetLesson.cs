using AutoMapper;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class GetLesson : IGetLesson
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly IStorageService _storage;

        public GetLesson(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, IStorageService storage)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _storage = storage;
        }

        public async Task<LessonResponse> Execute(long courseId, long moduleId, long id)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);

            if (lesson is null)
                throw new LessonException(ResourceExceptMessages.LESSON_DOESNT_EXISTS);

            var user = await _userService.GetUserInfos();

            if ((user is null && !lesson.isFree) ||
                (user is not null &&
                (await _uof.enrollmentRead.UserGotCourse(courseId, _sqids.Decode(user.id).Single())
                || lesson.isFree)))
                throw new LessonException(ResourceExceptMessages.LESSON_UNAUTHORIZED, System.Net.HttpStatusCode.Unauthorized);

            var response = _mapper.Map<LessonResponse>(lesson);
            response.VideoUrl = await _storage.GetVideo(lesson.Module.Course.courseIdentifier, lesson.VideoId);

            return response;
        }
    }
}
