using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

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

        public async Task<IList<CourseEntity>?> CoursesByTeacher(long userId) => await _dbContext.Courses.Where(d => d.TeacherId == userId).ToListAsync();

        public async Task<CourseEntity?> CourseById(long id) => await _dbContext.Courses.Include(d => d.Modules).SingleOrDefaultAsync(d => d.Id == id);

        public async Task<CourseEntity?> CourseByTeacherAndId(long userId, long id) => await _dbContext.Courses.Include(d => d.Modules).SingleOrDefaultAsync(d => d.TeacherId == userId && d.Id == id);

        public void UpdateCourse(CourseEntity course)
        {
            _dbContext.Courses.Update(course);
        }

        public IPagedList<CourseEntity> GetCourses(int page, GetCoursesFilterDto dto, int itemsQuantity = 6)
        {
            var courses = _dbContext.Courses.Where(d => d.Active);

            if (!string.IsNullOrEmpty(dto.Text))
                courses = courses.Where(d => dto.Text.Contains(d.Title));

            if (dto.Languages.Count != 0 && dto.Languages is not null)
                courses = courses.Where(d => dto.Languages.Contains(d.CourseLanguage));
            if (dto.Price is not null)
            {
                if((int)dto.Price == 0)
                {
                    courses = courses.Where(d => d.Price <= 50);
                } else if((int)dto.Price == 1)
                {
                    courses = courses.Where(d => d.Price >= 50 && d.Price <= 200);
                } else
                {
                    courses = courses.Where(d => d.Price >= 200);
                }               
            }

            return courses.Skip(page * itemsQuantity).Take(6).ToPagedList();
        }
    }
}
