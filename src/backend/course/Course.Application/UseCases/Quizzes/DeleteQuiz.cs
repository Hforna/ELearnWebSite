using Course.Application.UseCases.Repositories.Quizzes;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Quizzes
{
    public class DeleteQuiz : IDeleteQuiz
    {
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;

        public DeleteQuiz(SqidsEncoder<long> sqids, IUnitOfWork uof, IUserService userService)
        {
            _sqids = sqids;
            _uof = uof;
            _userService = userService;
        }

        public async Task Execute(long courseId, long quizId)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            var quiz = await _uof.quizRead.QuizById(quizId);

            if (quiz is null)
                throw new QuizException(ResourceExceptMessages.QUIZ_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            _uof.quizWrite.DeleteQuiz(quiz);
            await _uof.Commit();
        }
    }
}
