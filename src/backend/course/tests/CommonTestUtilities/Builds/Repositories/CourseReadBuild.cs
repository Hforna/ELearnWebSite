using Bogus;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories
{
    public class CourseReadBuild
    {
        private static Mock<ICourseReadOnly> _mock = new Mock<ICourseReadOnly>();
        public ICourseReadOnly Build()
        {
            return _mock.Object;
        }

        public void CourseByTeacherAndIdBuild(CourseEntity course, bool returnNull = false)
        {
            _mock.Setup(d => d.CourseByTeacherAndId(course.TeacherId, course.Id)).ReturnsAsync(returnNull ? null : course);
        }
    }
}
