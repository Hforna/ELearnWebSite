using AutoMapper;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Org.BouncyCastle.Asn1.Ocsp;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class UpdateLesson : IUpdateLesson
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public UpdateLesson(IUnitOfWork uof, IMapper mapper, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<LessonResponse> Execute(long courseId, long moduleId, long id, UpdateLessonRequest request)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);

            if (lesson is null)
                throw new LessonException(ResourceExceptMessages.LESSON_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var course = lesson.Module.Course;
            var module = lesson.Module;

            ValidateRequest(request, lesson, module);

            var requestModuleDecoded = _sqids.Decode(request.ModuleId).Single();
            var Requestedmodule = module.Id != requestModuleDecoded ? await _uof.moduleRead.ModuleById(requestModuleDecoded) : module;

            if (Requestedmodule is null)
                throw new ModuleException(ResourceExceptMessages.MODULE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            _mapper.Map(request, lesson);
            lesson.ModuleId = requestModuleDecoded;

            _uof.lessonWrite.UpdateLesson(lesson);

            if (Requestedmodule.Id == module.Id)
            {
                module.Lessons.Remove(lesson);
                module.Lessons.Insert(request.Order, lesson);
            } else
            {
                Requestedmodule.Lessons.Insert(request.Order, lesson);
                module.Duration -= lesson.Duration;
                Requestedmodule.Duration += lesson.Duration;

                _uof.moduleWrite.UpdateModule(Requestedmodule);
            }
            _uof.moduleWrite.UpdateModule(module);
            await _uof.Commit();

            return _mapper.Map<LessonResponse>(lesson);
        }

        void ValidateRequest(UpdateLessonRequest request, Lesson lesson, Module module)
        {
            if (request.Order > module.Lessons.Count || request.Order <= 0)
                throw new LessonException(ResourceExceptMessages.LESSON_POSITION_OUT_RANGE, System.Net.HttpStatusCode.BadRequest);

            var decodeRequestModule = _sqids.Decode(request.ModuleId);

            if (!decodeRequestModule.Any())
                throw new ModuleException(ResourceExceptMessages.MODULE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);
        }
    }
}
