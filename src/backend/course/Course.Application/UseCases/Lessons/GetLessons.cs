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
    public class GetLessons : IGetLessons
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storageService;

        public GetLessons(IMapper mapper, IUnitOfWork uof, IUserService userService, SqidsEncoder<long> sqids, IStorageService storageService)
        {
            _mapper = mapper;
            _uof = uof;
            _userService = userService;
            _sqids = sqids;
            _storageService = storageService;
        }

        public async Task<LessonsResponse> Execute(long moduleId)
        {
            var module = await _uof.moduleRead.ModuleById(moduleId);

            if (module is null)
                throw new LessonException(ResourceExceptMessages.MODULE_DOESNT_EXISTS);

            var lessons = module.Lessons;
            var user = await _userService.GetUserInfos();

            if (user is null || await _uof.enrollmentRead.UserGotCourse(module.CourseId, _sqids.Decode(user.id).Single()) == false)
                lessons.Where(d => d.isFree);

            var response = _mapper.Map<LessonsResponse>(lessons);
            var responseLessons = response.Lessons.Select(async lesson =>
            {
                lesson.VideoUrl = await _storageService.GetVideo(module.Course.courseIdentifier, lesson.VideoId);

                return lesson;
            });

            var lessonsList = await Task.WhenAll(responseLessons);

            response.ModuleId = _sqids.Encode(module.Id);
            response.Lessons = lessonsList.ToList();

            return response;
        }
    }
}
