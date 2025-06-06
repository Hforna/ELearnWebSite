﻿using Course.Domain.DTOs;
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
        public Task<decimal> CoursesNoteSum(long userId);
        public Task<int> GetQuantityUserCourse(long userId);
        public Task<IList<CourseEntity>?> CourseByIds(List<long> ids);
        public Task<IPagedList<CourseEntity>> CoursesUserHas(int page, int quantity, long userId);
        public Task<IList<CourseEntity>?> CoursesByTeacher(long userId);
        public Task<CourseEntity?> CourseByTeacherAndId(long userId, long id);
        public Task<IList<CourseEntity>?> GetNotActiveCourses();
        public Task<IList<CourseEntity>?> GetCoursesByIds(List<long> ids);
        public IPagedList<CourseEntity> TeacherCoursesPagination(int page, int quantity, long teacherId);
        public IPagedList<CourseEntity> GetCoursesPagination(int page, GetCoursesFilterDto dto, List<CourseEntity>? reccomendedCourses, int itemsQuantity = 6);
        public Task<List<CourseEntity>> GetCourseByUserVisitsAndMostVisited(GetCoursesFilterDto dto, List<long> mostVisitedCourses, List<string>? recommendedCourses = null);
    }
}
