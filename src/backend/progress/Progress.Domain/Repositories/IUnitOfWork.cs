using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public IQuizAttemptReadOnly quizAttemptRead { get; set; }
        public IQuizAttemptWriteOnly quizAttemptWrite { get; set; }
        public IGenericRepository genericRepository { get; set; }
        public IUserCourseProgressReadOnly userCourseProgressRead { get; set; }
        public IUserCourseProgressWriteOnly userCourseProgressWrite { get; set; }
        public IUserLessonProgressReadOnly userLessonProgressReadOnly { get; set; }
        public IUserLessonProgressWriteOnly userLessonProgressWriteOnly { get; set; }
        public Task Commit();
    }
}
