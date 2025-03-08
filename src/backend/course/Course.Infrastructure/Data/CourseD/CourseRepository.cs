using Azure.Core;
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

namespace Course.Infrastructure.Data.Course
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

        public async Task<CourseEntity?> CourseById(long id, bool asNoTracking = false)
        {
            var course = _dbContext.Courses
                .Include(d => d.Modules)
                .ThenInclude(d => d.Lessons)
                .Where(d => d.Id == id);

            if (asNoTracking)
                course = course.AsNoTracking();

            return await course.SingleOrDefaultAsync(d => d.Id == id);
        }

        public async Task<CourseEntity?> CourseByTeacherAndId(long userId, long id) => await _dbContext.Courses.Include(d => d.Modules).SingleOrDefaultAsync(d => d.TeacherId == userId && d.Id == id);

        public void UpdateCourse(CourseEntity course)
        {
            _dbContext.Courses.Update(course);
        }

        public async Task<IList<CourseEntity>?> GetNotActiveCourses()
        {
            return await _dbContext.Courses.Where(d => !d.Active).ToListAsync();
        }

        public void DeleteCourse(CourseEntity course)
        {
            _dbContext.Courses.Remove(course);
        }

        public void DeleteCourseRange(IList<CourseEntity> courses) => _dbContext.Courses.RemoveRange(courses);

        public async Task<IList<CourseEntity>?> CourseByIds(List<long> ids) => await _dbContext.Courses.Where(d => ids.Contains(d.Id) && d.Active).ToListAsync();

        public IPagedList<CourseEntity> GetCoursesPagination(int page, GetCoursesFilterDto dto, List<CourseEntity>? recommendedCourses = null, int itemsQuantity = 6)
        {
            var courses = _dbContext.Courses.Where(d => d.IsPublish);
            courses = FilterCourses(dto, courses);

            if (recommendedCourses is not null && recommendedCourses.Any())
                return recommendedCourses.Union(courses.Except(recommendedCourses))
                .OrderByDescending(d => d.totalVisits)
                .ToPagedList(page, itemsQuantity);
            return courses
                .OrderByDescending(d => d.totalVisits)
                .ToPagedList(page, itemsQuantity);
        }

        public async Task<List<CourseEntity>> GetCourseByUserVisitsAndMostVisited(GetCoursesFilterDto dto, List<long> mostVisitedCourses, List<string>? recommendedCourses = null)
        {
            var query = _dbContext.Courses.Where(d => d.IsPublish 
            && mostVisitedCourses.Contains(d.Id) && recommendedCourses.Contains(d.CourseType.ToString()));

            FilterCourses(dto, query);

            return await query.ToListAsync();
        }

        public async Task<IList<CourseEntity>?> GetCoursesByIds(List<long> ids)
        {
            return await _dbContext.Courses.Where(d => ids.Contains(d.Id)).ToListAsync();
        }

        IQueryable<CourseEntity> FilterCourses(GetCoursesFilterDto dto, IQueryable<CourseEntity> courses)
        {
            if (!string.IsNullOrEmpty(dto.Text))
                courses = courses.Where(d => d.Title.Contains(dto.Text) || d.Description.Contains(dto.Text));

            if (dto.Languages.Count != 0 && dto.Languages is not null)
                courses = courses.Where(d => dto.Languages.Contains(d.CourseLanguage));
            if (dto.Price is not null)
            {
                if ((int)dto.Price == 0)
                {
                    courses = courses.Where(d => d.Price <= 50);
                }
                else if ((int)dto.Price == 1)
                {
                    courses = courses.Where(d => d.Price >= 50 && d.Price <= 200);
                }
                else
                {
                    courses = courses.Where(d => d.Price >= 200);
                }
            }

            return courses;
        }

        public async Task<int> GetQuantityUserCourse(long userId)
        {
            return await _dbContext.Courses.CountAsync(d => d.TeacherId == userId && d.Active && d.Note > 0);
        }

        public async Task<decimal> CoursesNoteSum(long userId)
        {
            return await _dbContext.Courses.AsNoTracking().Where(d => d.TeacherId == userId && d.Active && d.Note > 0).SumAsync(d => d.Note);
        }

        public IPagedList<CourseEntity> TeacherCoursesPagination(int page, int quantity, long teacherId)
        {
            var query = _dbContext.Courses.Where(d => d.Active && d.TeacherId == teacherId);

            return query.ToPagedList(page, quantity);
        }
    }
}
