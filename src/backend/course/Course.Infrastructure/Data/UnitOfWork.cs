using Course.Domain.Repositories;
using Course.Infrastructure.Data.Course;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _dbContext;
        public ICourseReadOnly courseRead { get;  set; }
        public ICourseWriteOnly courseWrite { get;  set; }
        public IModuleWriteOnly moduleWrite { get;  set; }
        public IModuleReadOnly moduleRead { get; set; }
        public ILessonReadOnly lessonRead { get; set; }
        public ILessonWriteOnly lessonWrite { get; set; }
        public IVideoReadOnly videoRead { get; set; }
        public IVideoWriteOnly videoWrite { get; set; }
        public IEnrollmentReadOnly enrollmentRead { get; set; }
        public IEnrollmentWriteOnly enrollmentWrite { get; set; }
        public IReviewReadOnly reviewRead { get; set; }
        public IReviewWriteOnly reviewWrite { get; set; }

        public UnitOfWork(CourseDbContext dbContext, ICourseReadOnly courseRead, ICourseWriteOnly courseWrite,
            IModuleReadOnly moduleReadOnly, IModuleWriteOnly moduleWriteOnly, 
            ILessonReadOnly lessonRead, ILessonWriteOnly lessonWrite,
            IVideoReadOnly videoRead, IVideoWriteOnly videoWriteOnly,
            IEnrollmentReadOnly enrollmentRead, IEnrollmentWriteOnly enrollmentWrite,
            IReviewReadOnly reviewRead, IReviewWriteOnly reviewWrite)
        {
            _dbContext = dbContext;
            this.courseRead = courseRead;
            this.courseWrite = courseWrite;
            this.reviewRead = reviewRead;
            this.reviewWrite = reviewWrite;
            moduleRead = moduleReadOnly;
            moduleWrite = moduleWriteOnly;
            this.lessonRead = lessonRead;
            this.lessonWrite = lessonWrite;
            this.videoRead = videoRead;
            this.enrollmentRead = enrollmentRead;
            this.enrollmentWrite = enrollmentWrite;
            videoWrite = videoWriteOnly;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
