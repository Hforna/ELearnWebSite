using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Lessons
{
    public interface IDeleteLesson
    {
        public Task Execute(long courseId, long moduleId, long id);
    }
}
