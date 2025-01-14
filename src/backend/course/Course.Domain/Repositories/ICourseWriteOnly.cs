using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface ICourseWriteOnly
    {
        public void AddCourse(CourseEntity course);
        public void UpdateCourse(CourseEntity course);
        public void AddCourseTopics(IList<CourseTopicsEntity> topics);
    }
}
