using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Cache
{
    public interface ICourseCache
    {
        public Task<Dictionary<long, int>?> GetMostPopularCourses();
        public Task SetCourseOnMostVisited(long courseId);
    }
}
