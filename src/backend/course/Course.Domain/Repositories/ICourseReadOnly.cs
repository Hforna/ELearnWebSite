using Course.Domain.DTOs;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Domain.Repositories
{
    public interface ICourseReadOnly
    {
        public Task<CourseEntity?> CourseById(long id, bool asNoTracking = false);
        public Task<IList<CourseEntity>?> CourseByIds(List<long> ids);
        public Task<IList<CourseEntity>?> CoursesByTeacher(long userId);
        public Task<CourseEntity?> CourseByTeacherAndId(long userId, long id);
        public Task<IList<CourseEntity>?> GetNotActiveCourses();
        public IPagedList<CourseEntity> GetCourses(int page, GetCoursesFilterDto dto, List<string>? recommendedCourses = null, int itemsQuantity = 6);
    }
}
