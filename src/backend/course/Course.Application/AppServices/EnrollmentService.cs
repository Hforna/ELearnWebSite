using AutoMapper;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface IEnrollmentService
    {
        public Task<EnrollmentsResponse> GetCourseEnrollments(long id, int page, int quantity);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public EnrollmentService(IUnitOfWork uof, IMapper mapper, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<EnrollmentsResponse> GetCourseEnrollments(long id, int page, int quantity)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new UnauthorizedException(ResourceExceptMessages.COURSE_NOT_OF_USER);

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
