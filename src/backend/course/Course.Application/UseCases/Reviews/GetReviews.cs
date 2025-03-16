using AutoMapper;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Reviews
{
    public class GetReviews : IGetReviews
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;

        public GetReviews(IUnitOfWork uof, IMapper mapper, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
        }

        public async Task<ReviewsResponse> Execute(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var reviews = await _uof.reviewRead.ReviewsByCourse(courseId);

            var response = new ReviewsResponse();
            response.CourseId = _sqids.Encode(courseId);
            response.Reviews = reviews.Select(review =>
            {
                var response = _mapper.Map<ReviewResponse>(review);

                return response;
            }).ToList();

            return response;
        }
    }
}
