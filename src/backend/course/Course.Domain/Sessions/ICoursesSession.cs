using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Sessions
{
    public interface ICoursesSession
    {
        public Task AddCourseVisited(long id);
        public List<long>? GetCoursesVisited();
    }
}
