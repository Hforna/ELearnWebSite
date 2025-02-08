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

        public void AddLesson(Lesson lesson)
        {
            _dbContext.Lessons.Add(lesson);
        }

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

        public async Task<List<Lesson>?> LessonByModuleId(long moduleId)
        {
            return await _dbContext.Lessons.Where(d => d.Active && d.ModuleId == moduleId).ToListAsync();
        }
    }
}
