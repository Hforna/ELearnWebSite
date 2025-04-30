using Course.Communication.Requests;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Quizzes
{
    public interface ICreateQuiz
    {
        public Task<QuizResponse> Execute(CreateQuizRequest request);
    }
}
