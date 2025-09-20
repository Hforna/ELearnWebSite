using Progress.Application.UseCases.Interfaces.Progress;
using Progress.Domain.Entities;
using Progress.Domain.Exceptions;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Progress
{
    public class CompleteLesson : ICompleteLesson
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserRestService _userRest;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ICourseRestService _courseRest;

        public CompleteLesson(IUnitOfWork uof, IUserRestService userRest, SqidsEncoder<long> sqids, ICourseRestService courseRest)
        {
            _uof = uof;
            _userRest = userRest;
            _sqids = sqids;
            _courseRest = courseRest;
        }

        public async Task Execute(long lessonId)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var lessonInfos = await _courseRest.LessonInfos(lessonId);

            var lessonIdDeco = _sqids.Decode(lessonInfos.Id).Single();
            var courseId = _sqids.Decode(lessonInfos.CourseId).Single();
            var moduleId = _sqids.Decode(lessonInfos.ModuleId).Single();

            var userGotCourse = await _courseRest.UserGotCourse(lessonInfos.CourseId);

            if (!userGotCourse)
                throw new CourseException(ResourceExceptMessages.USER_DOESNT_GOT_COURSE, System.Net.HttpStatusCode.Unauthorized);

            var countLessons = await _courseRest.CountCourseLessons(lessonInfos.CourseId);

            var lessonProgress = new UserLessonProgress()
            {
                CourseId = courseId,
                LessonId = lessonIdDeco,
                IsCompleted = true,
                LastUpdate = DateTime.UtcNow,
                UserId = userId
            };

            var courseProgress = await _uof.userCourseProgressRead.GetUserCourseProgressByUserAndCourse(userId, courseId);
            courseProgress.TotalLessonsCompleted++;
            courseProgress.CompletionPercentage = courseProgress.TotalLessonsCompleted * 100 / countLessons;

            if(courseProgress.TotalLessonsCompleted == countLessons)
            {
                lessonProgress.IsCompleted = true;
                
            }

            _uof.genericRepository.Update<UserCourseProgress>(courseProgress);
            await _uof.genericRepository.Add<UserLessonProgress>(lessonProgress);
            await _uof.Commit();
        }
    }
}
