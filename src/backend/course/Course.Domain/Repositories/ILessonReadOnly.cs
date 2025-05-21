using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface ILessonReadOnly
    {
        public Task<Lesson?> LessonById(long id);
        public Task<List<Lesson>?> LessonByModuleId(long moduleId);
        public Task<List<Lesson>?> LessonsByModuleIdAndCourseId(long moduleId, long courseId);
        public Task<Lesson?> LessonByModuleIdAndCourseId(long moduleId, long courseId, long id);
        public Task<int> CountTotalLessons (long courseId);
    }
}
