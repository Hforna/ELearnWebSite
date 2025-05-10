using Progress.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectDbContext _dbContext;
        public IQuizAttemptReadOnly quizAttemptRead { get; set; }
        public IQuizAttemptWriteOnly quizAttemptWrite { get; set; }
        public IGenericRepository genericRepository { get; set; }
        public IUserCourseProgressReadOnly userCourseProgressRead { get; set; }
        public IUserCourseProgressWriteOnly userCourseProgressWrite { get; set; }
        public IUserLessonProgressReadOnly userLessonProgressReadOnly { get; set; }
        public IUserLessonProgressWriteOnly userLessonProgressWriteOnly { get; set; }

        public UnitOfWork(ProjectDbContext dbContext, IQuizAttemptWriteOnly quizAttemptWrite, 
            IQuizAttemptReadOnly quizAttemptRead, IGenericRepository genericRepository, 
            IUserCourseProgressReadOnly userCourseProgressRead, IUserCourseProgressWriteOnly userCourseProgressWrite,
            IUserLessonProgressReadOnly userLessonProgressRead, IUserLessonProgressWriteOnly userLessonProgressWrite)
        {
            _dbContext = dbContext;
            this.quizAttemptRead = quizAttemptRead;
            this.quizAttemptWrite = quizAttemptWrite;
            this.genericRepository = genericRepository;
            this.userCourseProgressWrite = userCourseProgressWrite;
            this.userCourseProgressRead = userCourseProgressRead;
            this.userLessonProgressReadOnly = userLessonProgressRead;
            this.userLessonProgressWriteOnly = userLessonProgressWrite;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
