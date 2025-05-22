using Progress.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Rest
{
    public interface ICourseRestService
    {
        public Task<bool> UserGotCourse(string courseId);
        public Task<QuizDto> GetQuiz(string quizId, string courseId);
        public Task<int> CountCourseLessons(string courseId);
        public Task<LessonInfosDto> LessonInfos(long lessonId);
    }
}
