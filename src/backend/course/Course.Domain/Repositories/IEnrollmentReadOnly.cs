using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Domain.Repositories
{
    public interface IEnrollmentReadOnly
    {
        public Task<bool> UserGotCourse(long courseId, long userId);
        public Task<List<long>> GetCourseUsersId(long courseId);
        public IPagedList<Enrollment> GetPagedEnrollments(long courseId, int page, int quantity);
        public Task<List<Enrollment>?> UserEnrollments(long userId);
    }
}
