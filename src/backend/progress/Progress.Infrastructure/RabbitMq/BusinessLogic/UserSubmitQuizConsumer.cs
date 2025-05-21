using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Progress.Domain.Dtos;
using Progress.Domain.Entities;
using Progress.Domain.Exceptions;
using Progress.Domain.RabbitMq;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Progress.Infrastructure.RabbitMq.BusinessLogic
{
    public class UserSubmitQuizConsumer : IUserSubmitQuizConsumer
    {
        private readonly IServiceProvider _serviceProvider;

        public UserSubmitQuizConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(string message)
        {
            var messageAsDto = JsonSerializer.Deserialize<UserSubmitDto>(message);
            var userId = messageAsDto.UserId;
            var attemptId = messageAsDto.AttemptId;
            var courseId = messageAsDto.CourseId;

            using(var scope = _serviceProvider.CreateScope())
            {
                var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var courseRest = scope.ServiceProvider.GetRequiredService<ICourseRestService>();

                var attempt = await uof.quizAttemptRead.QuizAttemptByUserAndId(userId, attemptId);

                if (attempt is null)
                    throw new QuizAttemptException(ResourceExceptMessages.QUIZ_ATTEMPT_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

                var userProgress = await uof.userCourseProgressRead.GetUserCourseProgressByUserAndCourse(userId, courseId);

                var sqids = scope.ServiceProvider.GetRequiredService<SqidsEncoder<long>>();

                var lessonsCount = await courseRest.CountCourseLessons(sqids.Encode(courseId));

                userProgress.TotalLessonsCompleted += 1;
                userProgress.CompletionPercentage = userProgress.TotalLessonsCompleted / lessonsCount * 100;
                userProgress.LastAccess = DateTime.UtcNow;
                userProgress.IsCompleted = userProgress.CompletionPercentage == 100;

                uof.genericRepository.Update<UserCourseProgress>(userProgress);
                await uof.Commit();
            }
        }
    }
}
