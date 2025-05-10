using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class UserGotCourse : IUserGotCourse
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;

        public UserGotCourse(IUserService userService, IUnitOfWork uof, SqidsEncoder<long> sqids)
        {
            _userService = userService;
            _uof = uof;
            _sqids = sqids;
        }

        public async Task<bool> Execute(GetCourseRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = _uof.courseRead.CourseById(_sqids.Decode(request.courseId).Single());

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(course.Id, userId);

            if (!userGotCourse)
                return false;
            return true;
        }
    }
}
