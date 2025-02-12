using Course.Communication.Requests;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Lessons
{
    public interface ICreateLesson
    {
        public Task<LessonResponse> Execute(CreateLessonRequest request, long moduleId, long courseId);
    }
}
