using CommonTestUtilities.Builds.Repositories.Executes;
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
            mock.Setup(d => d.enrollmentRead).Returns(new EnrollmentReadBuild().Build());
            mock.Setup(d => d.reviewRead).Returns(new ReviewReadBuild().Build());

            mock.Setup(d => d.courseWrite).Returns(CourseWriteBuild.Build());
            mock.Setup(d => d.reviewWrite).Returns(ReviewWriteBuild.Build());
            mock.Setup(d => d.enrollmentWrite).Returns(EnrollmentWriteBuild.Build());
            mock.Setup(d => d.moduleWrite).Returns(ModuleWriteBuild.Build());
            mock.Setup(d => d.lessonWrite).Returns(LessonWriteBuild.Build());
            mock.Setup(d => d.videoWrite).Returns(VideoWriteBuild.Build());
            return mock.Object;
        }
    }
}
