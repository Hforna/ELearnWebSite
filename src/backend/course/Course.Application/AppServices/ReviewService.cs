using AutoMapper;
using Course.Application.Services;
using Course.Application.Services.Validators.Review;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Exception;
using SharedMessages.CourseMessages;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface IReviewService
    {
        public Task<ReviewAnswerResponse> AnswerReview(long reviewId, CreateReviewResponseRequest request);
        public Task<ReviewResponse> CreateReview(CreateReviewRequest request, long courseId);
        public Task DeleteReview(long reviewId);
        public Task<ReviewResponse> GetReview(long reviewId);
        public Task<ReviewsResponse> GetReviews(long courseId);
    }

    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly EmailService _emailService;
        private readonly ILinkService _linkService;
        private readonly IProducerService _producerService;

        public ReviewService(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, 
            EmailService emailService, ILinkService linkService, IProducerService producerService)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _userService = userService;
            _emailService = emailService;
            _linkService = linkService;
            _producerService = producerService;
        }

        public async Task<ReviewAnswerResponse> AnswerReview(long reviewId, CreateReviewResponseRequest request)
        {
            if (request.Text.Length > 2000)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseById(review.CourseId);

            if (course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var reviewResponse = _mapper.Map<ReviewResponseEntity>(request);
            reviewResponse.CourseId = course.Id;
            reviewResponse.ReviewId = review.Id;
            reviewResponse.TeacherId = userId;

            await _uof.reviewWrite.AddReviewResponse(reviewResponse);
            await _uof.Commit();

            var customer = await _userService.GetUserInfosById(_sqids.Encode(review.CustomerId));

            await _emailService.SendEmail(customer.userName, customer.email, $"You review has just been answered by: {course.TeacherId}",
                $"Message: {request.Text}");

            var response = _mapper.Map<ReviewAnswerResponse>(reviewResponse);
            response.AddLink("reviews", _linkService.GenerateResourceLink("AnswerReview", new { reviewId = _sqids.Encode(reviewId) }), "GET");

            return response;
        }

        public async Task<ReviewResponse> CreateReview(CreateReviewRequest request, long courseId)
        {
            var validator = new CreateReviewValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);
            }

            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();

            if (user is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var userId = _sqids.Decode(user.id).Single();

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(courseId, userId);

            if (!userGotCourse)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var review = _mapper.Map<Review>(request);
            review.CourseId = courseId;
            review.CustomerId = userId;

            var courseReviewSum = await _uof.reviewRead.GetReviewSum(courseId);
            var courseReviewCount = await _uof.reviewRead.ReviewsCount(courseId);
            courseReviewCount += 1;
            courseReviewSum += (int)review.Rating;

            decimal noteAverage = courseReviewSum / courseReviewCount;
            var roundAverage = Math.Round(noteAverage, 1);
            course.Note = roundAverage;

            _uof.reviewWrite.Add(review);
            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            var courseNumber = await _uof.courseRead.GetQuantityUserCourse(course.TeacherId);
            var coursesSumNote = await _uof.courseRead.CoursesNoteSum(course.TeacherId);

            var sendCourseNoteMessage = new CourseNoteMessage()
            {
                CourseNumber = courseNumber,
                Note = course.Note,
                UserId = _sqids.Encode(course.TeacherId)
            };

            await _producerService.SendCourseNote(sendCourseNoteMessage);

            var response = _mapper.Map<ReviewResponse>(review);

            return response;
        }

        public async Task DeleteReview(long reviewId)
        {
            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var reviewOfUser = await _uof.reviewRead.ReviewOfUser(userId, reviewId);

            if (!reviewOfUser)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var course = await _uof.courseRead.CourseById(review.CourseId);

            var reviewSum = await _uof.reviewRead.GetReviewSum(review.CourseId) + (decimal)review.Rating;
            var reviewCount = await _uof.reviewRead.ReviewsCount(review.CourseId) - 1;
            var reviewAverage = reviewSum / reviewCount;
            var round = Math.Round(reviewAverage, 1);
            course.Note = round;

            _uof.reviewWrite.DeleteReview(review);
            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            var courseNumber = await _uof.courseRead.GetQuantityUserCourse(course.TeacherId);

            var sendCourseNoteMessage = new CourseNoteMessage()
            {
                CourseNumber = courseNumber,
                Note = course.Note,
                UserId = _sqids.Encode(course.TeacherId)
            };

            await _producerService.SendCourseNote(sendCourseNoteMessage);
        }

        public async Task<ReviewResponse> GetReview(long reviewId)
        {
            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = _mapper.Map<ReviewResponse>(review);

            return response;
        }

        public async Task<ReviewsResponse> GetReviews(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

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
