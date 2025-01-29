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


            var courses = _uof.courseRead.GetCourses(page, filterDto, itemsQuantity);

            var getCoursesList = _coursesSession.GetCoursesVisited();

            var quantityCourseType = new Dictionary<string, int>();

            foreach(var id in getCoursesList)
            {
                var course = await _uof.courseRead.CourseById(id);

                quantityCourseType[course.CourseType.ToString()] = 
                    quantityCourseType.GetValueOrDefault(course.CourseType.ToString(), 0) + 1;
            }

            var quantityTypeOrdered = quantityCourseType.OrderByDescending(d => d.Value);

            var coursesOrder = new List<CourseEntity>();

            foreach(var type in quantityTypeOrdered)
            {
                coursesOrder.AddRange(courses.Where(d => d.CourseType.ToString() == type.Key));
            }
            coursesOrder.AddRange(courses.Where(d => quantityCourseType.ContainsKey(d.CourseType.ToString()) == false));

            var coursesToResponse = coursesOrder.Select(async course =>
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
