using Progress.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Repositories
{
    public interface IUserLessonProgressReadOnly
    {
        public Task<List<UserLessonProgress>?> GetByUserId(long userId);
    }
}
