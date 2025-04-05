using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IEnrollmentWriteOnly
    {
        public Task AddEnrollment(Enrollment enrollment);
        public void DeleteEnrollment(Enrollment enrollment);
        public void DeleteEnrollmentsRange(List<Enrollment> enrollments);
    }
}
