using Course.Application.UseCases.Repositories.Lessons;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class DeleteLesson : IDeleteLesson
    {
        private readonly IUnitOfWork _uof;
        private readonly IStorageService _storage;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public DeleteLesson(IUnitOfWork uof, IStorageService storage, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _storage = storage;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task Execute(long courseId, long moduleId, long id)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);

            if (lesson is null)
                throw new LessonException(ResourceExceptMessages.LESSON_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (lesson.Module.Course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            var video = await _uof.videoRead.VideoById(lesson.VideoId);

            if (video is null)
                return;

            var course = lesson.Module.Course;
            var module = lesson.Module;

            await _storage.DeleteVideo(course.courseIdentifier, video.Id);

            course.Duration -= lesson.Duration;
            module.Duration -= lesson.Duration;

            await _uof.videoWrite.DeleteVideo(video.Id);
            _uof.lessonWrite.DeleteLesson(lesson);
            _uof.courseWrite.UpdateCourse(course);
            _uof.moduleWrite.UpdateModule(module);

            await _uof.Commit();
        }
    }
}
