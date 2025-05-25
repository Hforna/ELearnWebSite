using Progress.Application.Responses;
using Progress.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Interfaces.Progress
{
    public interface ICourseProgress
    {
        public Task<UserCourseProgressResponse> Execute(long courseId);
    }
}
