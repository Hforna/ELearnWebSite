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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public class CoursesThatUserBought : ICourseThatUserBought
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storageService;

        public CoursesThatUserBought(IMapper mapper, IUnitOfWork uof, 
            IUserService userService, SqidsEncoder<long> sqids, IStorageService storageService)
        {
            _mapper = mapper;
            _uof = uof;
            _userService = userService;
            _sqids = sqids;
            _storageService = storageService;
        }

        public async Task<CoursesPaginationResponse> Execute(int page, int quantity)
        {
            var user = await _userService.GetUserInfos();

            if (user is null)
                throw new UserException(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);

            var userId = _sqids.Decode(user.id).Single();

            var userCourses = await _uof.courseRead.CoursesUserHas(page, quantity, userId);

            var coursesSelect = userCourses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);

                return response;
            }).ToList();

            var coursesResponse = await Task.WhenAll(coursesSelect);

            return new CoursesPaginationResponse()
            {
                Count = userCourses.Count,
                Courses = coursesResponse.ToList(),
                IsFirstPage = userCourses.IsFirstPage,
                IsLastPage = userCourses.IsLastPage,
                PageNumber = userCourses.PageNumber,
                TotalItemCount = userCourses.TotalItemCount
            };
        }
    }
}
