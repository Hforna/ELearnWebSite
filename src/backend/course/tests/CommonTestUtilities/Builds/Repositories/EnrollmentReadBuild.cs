using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace CommonTestUtilities.Builds.Repositories
{
    public class EnrollmentReadBuild
    {
        private static Mock<IEnrollmentReadOnly> _mock = new Mock<IEnrollmentReadOnly>();

        public IEnrollmentReadOnly Build()
        {
            return _mock.Object;
        }

        public void UserGotCourse(long courseId, long userId, bool gotCourse = true)
        {
            _mock.Setup(d => d.UserGotCourse(courseId, userId)).ReturnsAsync(gotCourse);
        }

        public void GetPagedEnrollments(int page, int quantity, long courseId, List<Enrollment> enrollments)
        {
            _mock.Setup(d => d.GetPagedEnrollments(courseId, page, quantity)).Returns(enrollments.ToPagedList(page, quantity));
        }
    }

}
