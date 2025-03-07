using AutoMapper;
using Course.Application.Services.Validators.Review;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Enums;
using Course.Communication.Requests;
using Course.Communication.Requests.MessageSenders;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Reviews
{
    public class CreateReview : ICreateReview
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserSenderService _userSenderService;

        public CreateReview(IUnitOfWork uof, IMapper mapper, IUserService userService, 
            SqidsEncoder<long> sqids, IUserSenderService userSenderService)
        {
            _uof = uof;
            _userSenderService = userSenderService;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<ReviewResponse> Execute(CreateReviewRequest request, long courseId)
        {
            Validate(request);

            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var user = await _userService.GetUserInfos();
            
            if (user is null)
                throw new UserException(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS, System.Net.HttpStatusCode.Unauthorized);

            var userId = _sqids.Decode(user.id).Single();

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(courseId, userId);

            if (!userGotCourse)
                throw new CourseException(ResourceExceptMessages.USER_DOESNT_GOT_COURSE, System.Net.HttpStatusCode.Unauthorized);

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

            var sendCourseNoteMessage = new SendCourseNoteMessage()
            {
                CourseNumber = courseNumber,
                Note = course.Note,
                UserId = _sqids.Encode(course.TeacherId)
            };

            await _userSenderService.SendCourseNote(sendCourseNoteMessage);

            var response = _mapper.Map<ReviewResponse>(review);

            return response;
        }

        void Validate(CreateReviewRequest request)
        {
            var validator = new CreateReviewValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new ReviewException(errorMessages, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
