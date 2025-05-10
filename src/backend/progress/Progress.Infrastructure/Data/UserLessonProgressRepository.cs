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
    public class UserLessonProgressRepository : IUserLessonProgressReadOnly, IUserLessonProgressWriteOnly
    {
        private readonly ProjectDbContext _dbContext;

        public UserLessonProgressRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserLessonProgress>?> GetByUserId(long userId)
        {
            return await _dbContext.UserLessonProgresses.Where(d => d.UserId == userId).ToListAsync();
        }
    }
}
