using Progress.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Repositories
{
    public interface IQuizAttemptReadOnly
    {
        public Task<QuizAttempts?> QuizAttemptPendingByUserId(long userId);
        public Task<List<QuizAttempts>?> UserQuizAttempts(long userId);
        public Task<QuizAttempts?> QuizAttemptByUserAndId(long userId, Guid quizAttempt);
    }
}
