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
    public class DeleteQuestion : IDeleteQuestion
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public DeleteQuestion(IUnitOfWork uof, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task Execute(long quizId, long questionId)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var question = await _uof.quizRead.QuestionByIdAndQuiz(quizId, questionId);

            if (question is null)
                throw new QuizException(ResourceExceptMessages.QUIZ_OR_QUESTION_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

            if (question.Quiz.Course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            _uof.quizWrite.DeleteQuestion(question);
            await _uof.Commit();
        }
    }
}
