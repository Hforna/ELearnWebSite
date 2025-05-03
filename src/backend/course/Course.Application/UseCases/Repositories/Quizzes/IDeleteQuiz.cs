using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Quizzes
{
    public interface IDeleteQuiz
    {
        public Task Execute(long courseId, long quizId);
    }
}
