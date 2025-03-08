using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Course
{
    public interface ITeacherCourses
    {
        public Task<CoursesPaginationResponse> Execute(int page, int quantity, long teacherId);
    }
}
