using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Lessons
{
    public interface IGetLesson
    {
        public Task<LessonResponse> Execute(long courseId, long moduleId, long id);
    }
}
