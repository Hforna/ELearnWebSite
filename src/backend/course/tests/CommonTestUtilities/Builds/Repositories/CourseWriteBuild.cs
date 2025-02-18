using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories
{
    public class CourseWriteBuild
    {
        public static ICourseWriteOnly Build()
        {
            var mock = new Mock<ICourseWriteOnly>();

            return mock.Object;
        }
    }
}
