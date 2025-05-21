using Progress.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Repositories
{
    public interface IUserCourseProgressReadOnly
    {
        public Task<List<UserCourseProgress>?> GetUserCourseProgressByUser(long userId);
        public Task<UserCourseProgress?> GetUserCourseProgressByUserAndCourse(long userId, long courseId);
    }
}
