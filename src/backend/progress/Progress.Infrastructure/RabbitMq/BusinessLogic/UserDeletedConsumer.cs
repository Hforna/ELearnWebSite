using Microsoft.Extensions.DependencyInjection;
using Progress.Domain.Dtos.RabbitMqMessages;
using Progress.Domain.Entities;
using Progress.Domain.RabbitMq;
using Progress.Domain.Repositories;
using Progress.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Progress.Infrastructure.RabbitMq.ConsumerLogic
{
    public class UserDeletedConsumer : IUserDeletedConsumer
    {
        private readonly IServiceProvider _serviceProvider;

        public UserDeletedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(string message)
        {
            var messageSerialized = JsonSerializer.Deserialize<UserDeletedDto>(message);

            using(var scope = _serviceProvider.CreateScope())
            {
                var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var quizAttempts = await uof.quizAttemptRead.UserQuizAttempts(messageSerialized.userId);
                var userCourseProgress = await uof.userCourseProgressRead.GetUserCourseProgressByUser(messageSerialized.userId);
                var userLessonProgress = await uof.userLessonProgressReadOnly.GetByUserId(messageSerialized.userId);

                if (userCourseProgress is not null)
                    uof.genericRepository.DeleteRange<UserCourseProgress>(userCourseProgress);

                if (userLessonProgress is not null)
                    uof.genericRepository.DeleteRange<UserLessonProgress>(userLessonProgress);

                if (quizAttempts is not null)
                    uof.genericRepository.DeleteRange<QuizAttempts>(quizAttempts);

                await uof.Commit();
            }
        }
    }
}
