using AutoMapper;
using Course.Application.Services;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using MediaInfo.DotNetWrapper;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Reviews
{
    public class AnswerReview : IAnswerReview
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly EmailService _emailService;
        private readonly ILinkService _linkService;

        public AnswerReview(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, 
            EmailService emailService, ILinkService linkService)
        {
            _uof = uof;
            _mapper = mapper;
            _linkService = linkService;
            _sqids = sqids;
            _userService = userService;
            _emailService = emailService;
        }

        public async Task<ReviewAnswerResponse> Execute(long reviewId, CreateReviewResponseRequest request)
        {
            if (request.Text.Length > 2000)
                throw new ReviewException(ResourceExceptMessages.TEXT_LENGTH_MUST_BE_LESS_THAN_300, System.Net.HttpStatusCode.BadRequest);

            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new ReviewException(ResourceExceptMessages.REVIEW_NOT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseById(review.CourseId);

            if (course.TeacherId != userId)
                throw new UserException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            var reviewResponse = _mapper.Map<ReviewResponseEntity>(request);
            reviewResponse.CourseId = course.Id;
            reviewResponse.ReviewId = review.Id;
            reviewResponse.TeacherId = userId;

            await _uof.reviewWrite.AddReviewResponse(reviewResponse);
            await _uof.Commit();

            var response = _mapper.Map<ReviewAnswerResponse>(reviewResponse);
            response.AddLink("reviews", _linkService.GenerateResourceLink("AnswerReview", new { reviewId = _sqids.Encode(reviewId) }), "GET");

            return response;
        }
    }
}
