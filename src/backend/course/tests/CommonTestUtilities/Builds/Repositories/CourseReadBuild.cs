using Bogus;
using CommonTestUtilities.Entities;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace CommonTestUtilities.Builds.Repositories
{
    public class CourseReadBuild
    {
        private static Mock<ICourseReadOnly> _mock = new Mock<ICourseReadOnly>();
        public ICourseReadOnly Build()
        {
            return _mock.Object;
        }

        public void CourseId(CourseEntity course, bool returnNull = false)
        {
            _mock.Setup(d => d.CourseById(course.Id, false)).ReturnsAsync(returnNull ? null : course);
        }

        public void CourseByIds(List<long> ids, List<CourseEntity> courses)
        {
            _mock.Setup(d => d.CourseByIds(ids)).ReturnsAsync(courses);
        }

        public void CourseByTeacherAndIdBuild(CourseEntity course, bool returnNull = false)
        {
            _mock.Setup(d => d.CourseByTeacherAndId(course.TeacherId, course.Id)).ReturnsAsync(returnNull ? null : course);
        }

        public void GetCourseByUserVisitsAndMostVisited(GetCoursesFilterDto request, List<long> mostVisitedCourses, List<CourseEntity> courses)
        {
            _mock.Setup(d => d.GetCourseByUserVisitsAndMostVisited(It.IsAny<GetCoursesFilterDto>(),
            It.IsAny<List<long>>(),
            It.IsAny<List<string>>())).ReturnsAsync(courses);
        }

        public void GetCoursesPagination(int page, int quantityItems, GetCoursesFilterDto dto, List<CourseEntity> courses, List<CourseEntity> recommendedCourses)
        {
            _mock.Setup(d => d.GetCoursesPagination(It.IsAny<int>(), 
                It.IsAny<GetCoursesFilterDto>(), It.IsAny<List<CourseEntity>>(), It.IsAny<int>()))
                .Returns(courses.ToPagedList(page, quantityItems));
        }
    }
}
