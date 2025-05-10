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
    public class UserCourseProgressRepository : IUserCourseProgressReadOnly, IUserCourseProgressWriteOnly
    {
        private readonly ProjectDbContext _dbContext;

        public UserCourseProgressRepository(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserCourseProgress>?> GetUserCourseProgressByUser(long userId)
        {
            return await _dbContext.UserCourseProgresses.Where(d => d.UserId == userId).ToListAsync();
        }
    }
}
