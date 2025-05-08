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
        public Task<bool> UserGotQuiz(string quizId);
        public Task<QuizDto> GetQuiz(string quizId, string courseId);
    }
}
