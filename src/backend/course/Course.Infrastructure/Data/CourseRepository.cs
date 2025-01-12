using Course.Domain.Entitites;
using Course.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data
{
    public class CourseRepository : ICourseReadOnly, ICourseWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public CourseRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public void AddCourse(CourseEntity course)
        {
            _dbContext.Courses.Add(course);
        }

        public void AddCourseTopics(IList<CourseTopicsEntity> topics)
        {
            _dbContext.CourseTopics.AddRange(topics);
        }
    }
}
