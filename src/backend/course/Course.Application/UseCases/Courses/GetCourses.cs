using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Sessions;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Course
{
    public class GetCourses : IGetCourses
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ICoursesSession _coursesSession;

        public GetCourses(IUnitOfWork uof, IMapper mapper, 
            IStorageService storageService, SqidsEncoder<long> sqids, ICoursesSession coursesSession)
        {
            _uof = uof;
            _mapper = mapper;
            _storageService = storageService;
            _sqids = sqids;
            _coursesSession = coursesSession;
        }

        public async Task<CoursesResponse> Execute(GetCoursesRequest request, int page, int itemsQuantity)
        {
            var filterDto = _mapper.Map<GetCoursesFilterDto>(request);

            var getCoursesList = _coursesSession.GetCoursesVisited();

            List<string>? coursesTypes = null;

            if(getCoursesList is not null)
            {
                var visitedCourses = await _uof.courseRead.CourseByIds(getCoursesList);
                coursesTypes = visitedCourses!
                    .GroupBy(d => d.CourseType)
                    .ToDictionary(d => d.Key.ToString(), g => g.Count())
                    .OrderByDescending(d => d.Value)
                    .Select(d => d.Key).ToList();
            }

            var courses = _uof.courseRead.GetCourses(page, filterDto, coursesTypes, itemsQuantity);

            var coursesToResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                //response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                response.CourseId = _sqids.Encode(course.Id);
                response.TeacherId = _sqids.Encode(course.TeacherId);

                return response;
            });

            var coursesResponse = await Task.WhenAll(coursesToResponse);

            var response = new CoursesResponse()
            {
                Count = courses.Count,
                Courses = coursesResponse.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };

            return response;
        }
    }
}
