using Course.Domain.Entitites.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IQuizWriteOnly
    {
        public Task Add(QuizEntity quiz);
    }
}
