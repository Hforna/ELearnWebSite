using Microsoft.EntityFrameworkCore;
using Progress.Domain.Entities;
using Progress.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.Data
{
    public class QuizAttemptRepository : IQuizAttemptReadOnly, IQuizAttemptWriteOnly
    {
        private readonly ProjectDbContext _dbContext;

        public QuizAttemptRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QuizAttempts?> QuizAttemptByUserAndId(long userId, Guid quizAttempt)
        {
            return await _dbContext.QuizAttempts.SingleOrDefaultAsync(d => d.UserId == userId && d.Id == quizAttempt);
        }

        public async Task<QuizAttempts?> QuizAttemptPendingByUserId(long userId)
        {
            return await _dbContext.QuizAttempts.SingleOrDefaultAsync(d => d.UserId == userId && d.Status == Domain.Enums.QuizAttemptStatusEnum.PEDNING);
        }

        public async Task<List<QuizAttempts>?> UserQuizAttempts(long userId)
        {
            return await _dbContext.QuizAttempts.Where(d => d.UserId == userId).ToListAsync();
        }
    }
}
