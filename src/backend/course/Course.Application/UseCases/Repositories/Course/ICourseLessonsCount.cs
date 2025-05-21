using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Course
{
    public interface ICourseLessonsCount
    {
        public Task<int> Execute(long courseId);
    }
}
