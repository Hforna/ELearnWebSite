using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class GetTenMostPopularWeekCourses : IGetTenMostPopularWeekCourses
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly ICourseCache _courseCache;
        private readonly IStorageService _storageService;
        private readonly SqidsEncoder<long> _sqids;

        public GetTenMostPopularWeekCourses(IMapper mapper, IUnitOfWork uof, 
            ICourseCache courseCache, IStorageService storageService, SqidsEncoder<long> sqids)
        {
            _mapper = mapper;
            _uof = uof;
            _courseCache = courseCache;
            _storageService = storageService;
            _sqids = sqids;
        }

        public async Task<CoursesResponse> Execute()
        {
            var cacheCourses = await _courseCache.GetMostPopularCourses();
            cacheCourses = cacheCourses.Take(10).ToDictionary(k => k.Key, v => v.Value);
            
            var courses = await _uof.courseRead.CourseByIds(cacheCourses.Keys.ToList());

            var courseTasks = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                return response;
            }).ToList();

            var taskCourseResponse = await Task.WhenAll(courseTasks);

            return new CoursesResponse { Courses = taskCourseResponse.ToList() };
        }
    }
}
