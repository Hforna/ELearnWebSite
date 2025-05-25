using AutoMapper;
using Progress.Application.Responses;
using Progress.Application.UseCases.Interfaces.Progress;
using Progress.Domain.Exceptions;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.UseCases.Progress
{
    public class CourseProgress : ICourseProgress
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserRestService _userRest;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        public CourseProgress(IUnitOfWork uof, IUserRestService userRest,
            ICourseRestService courseRest, SqidsEncoder<long> sqids, IMapper mapper)
        {
            _userRest = userRest;
            _mapper = mapper;
            _sqids = sqids;
            _uof = uof;
            _mapper = mapper;
        }

        public async Task<UserCourseProgressResponse> Execute(long courseId)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userProgress = await _uof.userCourseProgressRead.GetUserCourseProgressByUserAndCourse(userId, courseId);

            if (userProgress is null)
                throw new CourseException(ResourceExceptMessages.USER_DOESNT_GOT_COURSE, System.Net.HttpStatusCode.Unauthorized);

            var response = _mapper.Map<UserCourseProgressResponse>(userProgress);

            return response;
        }
    }
}
