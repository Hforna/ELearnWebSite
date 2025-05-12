using AutoMapper;
using Course.Application.UseCases.Repositories.Reviews;
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

namespace Course.Application.UseCases.Reviews
{
    public class DeleteReview : IDeleteReview
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ISenderService _userSender;

        public DeleteReview(IUnitOfWork uof, IUserService userService, 
            SqidsEncoder<long> sqids, ISenderService userSender)
        {
            _uof = uof;
            _userSender = userSender;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task Execute(long reviewId)
        {
            var review = await _uof.reviewRead.ReviewById(reviewId);

            if (review is null)
                throw new ReviewException(ResourceExceptMessages.REVIEW_NOT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var reviewOfUser = await _uof.reviewRead.ReviewOfUser(userId, reviewId);

            if (!reviewOfUser)
                throw new ReviewException(ResourceExceptMessages.REVIEW_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

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

            await _userSender.SendCourseNote(sendCourseNoteMessage);
        }
    }
}
