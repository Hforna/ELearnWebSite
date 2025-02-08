using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Lessons
{
    public interface IGetLessons
    {
        public Task<LessonsResponse> Execute(long moduleId);
    }
}
