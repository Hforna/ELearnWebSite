using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Quizzes
{
    public interface IDeleteQuestion
    {
        public Task Execute(long quizId, long questionId);
    }
}
