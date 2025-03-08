using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class TeacherCourses : ITeacherCourses
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storageService;

        public TeacherCourses(IMapper mapper, IUnitOfWork uof, 
            SqidsEncoder<long> sqids, IStorageService storageService)
        {
            _mapper = mapper;
            _uof = uof;
            _sqids = sqids;
            _storageService = storageService;
        }

        public async Task<CoursesPaginationResponse> Execute(int page, int quantity, long teacherId)
        {
            var courses = _uof.courseRead.TeacherCoursesPagination(page, quantity, teacherId);

            if (courses is null)
                throw new CourseException(ResourceExceptMessages.TEACHER_DOESNT_HAVE_COURSES, System.Net.HttpStatusCode.NotFound);

            var coursesResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);

                return response;
            });

            var courseResponseList = await Task.WhenAll(coursesResponse);

            return new CoursesPaginationResponse()
            {
                Count = courses.Count,
                Courses = courseResponseList.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };
        }
    }
}
