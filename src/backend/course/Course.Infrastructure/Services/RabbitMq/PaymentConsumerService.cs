using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using MassTransit;
using MassTransit.SagaStateMachine;
using SharedMessages.PaymentMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sqids;
using Course.Domain.Entitites;
using Course.Domain.Services.RabbitMq;

namespace Course.Infrastructure.Services.RabbitMq
{
    public class PaymentConsumerService : IConsumer<AllowCourseToUserMessage>, IConsumer<UserGotRefundMessage>
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserSenderService _userSender;

        public PaymentConsumerService(IUnitOfWork uof, IUserService userService, SqidsEncoder<long> sqids, IUserSenderService userSender)
        {
            _uof = uof;
            _userSender = userSender;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task Consume(ConsumeContext<AllowCourseToUserMessage> context)
        {
            var user = await _userService.GetUserInfosById(_sqids.Encode(context.Message.UserId));
            var courses = await _uof.courseRead.GetCoursesByIds(context.Message.CoursesIds);

            if(courses is not null)
            {
                foreach (var course in courses)
                {
                    var enrollment = new Enrollment()
                    {
                        Active = true,
                        CourseId = course.Id,
                        CustomerId = context.Message.UserId
                    };
                    course.Enrollments = course.Enrollments is not null ? course.Enrollments + 1 : 1;

                    _uof.courseWrite.UpdateCourse(course);
                    await _uof.enrollmentWrite.AddEnrollment(enrollment);

                    await _uof.Commit();
                }
            }
            
        }

        public async Task Consume(ConsumeContext<UserGotRefundMessage> context)
        {
            var userId = context.Message.UserId;

            var userEnrollments = await _uof.enrollmentRead.GetEnrollmentsByUserIdAndCoursesIds(userId, context.Message.CourseIds);

            foreach(var course in userEnrollments.Select(d => d.Course))
            {
                course.Enrollments -= 1;

                var userReviews = await _uof.reviewRead.UserReviews(userId, course.Id);

                if(userReviews is not null)
                {
                    var sumNotes = userReviews.Sum(d => (int)d.Rating);

                    var courseNotesCount = await _uof.reviewRead.ReviewsCount(course.Id);
                    var courseSum = await _uof.reviewRead.GetReviewSum(course.Id);

                    courseNotesCount -= userReviews.Count;
                    courseSum -= sumNotes;

                    var calcNote = courseSum / courseNotesCount;
                    var roundNote = Math.Round(calcNote, 2);
                    course.Note = roundNote;

                    var courseNumber = await _uof.courseRead.GetQuantityUserCourse(userId);

                    await _userSender.SendCourseNote(new SharedMessages.CourseMessages.CourseNoteMessage()
                    {
                        CourseNumber = courseNumber,
                        Note = roundNote,
                        UserId = _sqids.Encode(course.TeacherId)
                    });

                    _uof.reviewWrite.DeleteReviewsRange(userReviews);
                }
                _uof.courseWrite.UpdateCourse(course);
            }
            _uof.enrollmentWrite.DeleteEnrollmentsRange(userEnrollments);
            await _uof.Commit();
        }
    }
}
