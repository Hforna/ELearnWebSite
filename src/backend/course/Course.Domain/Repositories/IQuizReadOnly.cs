using Course.Domain.Entitites.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IQuizReadOnly
    {
        public Task<QuizEntity?> QuizById(long quizId);
        public Task<List<QuestionEntity>?> QuestionsByQuiz(long quizId);
        public Task<List<AnswerOption>?> AnswerOptionsByQuestion(long questionId);
        public Task<QuestionEntity?> QuestionByIdAndQuiz(long quizId, long questionId);
    }
}
