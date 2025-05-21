using Course.Application.UseCases.Repositories.Course;
using Course.Domain.Repositories;
using Course.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class CourseLessonsCount : ICourseLessonsCount
    {
        private readonly IUnitOfWork _uof;

        public CourseLessonsCount(IUnitOfWork uof) => _uof = uof;

        public async Task<int> Execute(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var countLessons = await _uof.lessonRead.CountTotalLessons(courseId);
            var countQuizzes = await _uof.quizRead.CountQuizzes(courseId);

            return countLessons + countQuizzes;
        }
    }
}
