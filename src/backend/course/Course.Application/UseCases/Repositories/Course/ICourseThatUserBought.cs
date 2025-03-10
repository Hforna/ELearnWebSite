using Course.Communication.Responses;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Application.UseCases.Repositories.Course
{
    public interface ICourseThatUserBought
    {
        public Task<CoursesPaginationResponse> Execute(int page, int quantity);
    }
}
