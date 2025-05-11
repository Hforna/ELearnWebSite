using Progress.Application.Requests;
using Progress.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Interfaces
{
    public interface ISubmitQuizAttempt
    {
        public Task<QuizSubmitResponse> Execute(SubmitQuizRequest request);
    }
}
