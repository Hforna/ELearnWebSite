using AutoMapper;
using Course.Application.UseCases.Repositories.Enrollments;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Course.Application.UseCases.Enrollments
{
    public class GetCourseEnrollments : IGetCourseEnrollments
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public GetCourseEnrollments(IUnitOfWork uof, IMapper mapper, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<EnrollmentsResponse> Execute(long id, int page, int quantity)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            var enrollments = _uof.enrollmentRead.GetPagedEnrollments(id, page, quantity);

            var enrollmentsResponse = enrollments.Select(enrollment =>
            {
                var response = _mapper.Map<EnrollmentResponse>(enrollment);
                response.CourseId = _sqids.Encode(enrollment.CourseId);
                response.Id = _sqids.Encode(enrollment.Id);
                response.CustomerId = _sqids.Encode(enrollment.CustomerId);

                return response;
            }).ToList();

            var response = new EnrollmentsResponse()
            {
                Enrollments = enrollmentsResponse,
                Count = enrollments.Count,
                IsFirstPage = enrollments.IsFirstPage,
                IsLastPage = enrollments.IsLastPage,
                PageNumber = enrollments.PageNumber,
                TotalItemCount = enrollments.TotalItemCount
            };

            return response;
        }
    }
}
