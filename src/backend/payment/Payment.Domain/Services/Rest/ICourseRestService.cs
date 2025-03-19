using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Services.Rest
{
    public interface ICourseRestService
    {
        public Task<CourseDto?> GetCourse(string courseId);
    }
}
