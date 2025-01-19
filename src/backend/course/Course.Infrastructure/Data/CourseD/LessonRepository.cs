using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.Course
{
    public class LessonRepository : ILessonReadOnly, ILessonWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public LessonRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public void DeleteLesson(Lesson lesson)
        {
            _dbContext.Lessons.Remove(lesson);
        }

        public void DeleteLessonRange(IList<Lesson> lessons)
        {
            _dbContext.Lessons.RemoveRange(lessons);
        }

        public async Task<Lesson?> LessonById(long id)
        {
            return await _dbContext.Lessons.SingleOrDefaultAsync(d => d.Id == id);
        }
    }
}
