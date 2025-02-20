using Bogus;
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
        public static ICourseReadOnly Build() => _mock.Object;
    }
}
