using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Repositories
{
    public static class UnitOfWorkBuild
    {
    
        public static IUnitOfWork Build(Mock<ICourseReadOnly>? courseReadMock = null)
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(d => d.courseRead).Returns(courseReadMock is null ? new CourseReadBuild().Build() : courseReadMock.Object);
            mock.Setup(d => d.courseWrite).Returns(CourseWriteBuild.Build());
            return mock.Object;
        }
    }
}
