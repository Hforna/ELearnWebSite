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

namespace Course.Infrastructure.Services.RabbitMq
{
    public class PaymentConsumerService : IConsumer<AllowCourseToUserMessage>
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public PaymentConsumerService(IUnitOfWork uof, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
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
    }
}
