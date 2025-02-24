using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Application.UseCases.Repositories.Enrollments
{
    public interface IGetCourseEnrollments
    {
        public Task<EnrollmentsResponse> Execute(long id, int page, int quantity);
    }
}
