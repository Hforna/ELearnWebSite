using Course.Communication.Requests;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Course
{
    public interface IUpdateCourse
    {
        public Task<CourseShortResponse> Execute(long id, UpdateCourseRequest request);
    }
}
