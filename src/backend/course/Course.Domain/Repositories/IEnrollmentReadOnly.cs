using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IEnrollmentReadOnly
    {
        public Task<bool> UserGotCourse(long courseId, long userId);
    }
}
