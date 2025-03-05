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
    
        public static IUnitOfWork Build(
            ICourseReadOnly? courseReadMock = null,
            IReviewReadOnly? reviewReadMock = null,
            IEnrollmentReadOnly? enrollmentRead = null)
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(d => d.courseRead).Returns(courseReadMock ?? new CourseReadBuild().Build());
            mock.Setup(d => d.enrollmentRead).Returns(enrollmentRead ?? new EnrollmentReadBuild().Build());
            mock.Setup(d => d.reviewRead).Returns(reviewReadMock ?? new ReviewReadBuild().Build());

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
