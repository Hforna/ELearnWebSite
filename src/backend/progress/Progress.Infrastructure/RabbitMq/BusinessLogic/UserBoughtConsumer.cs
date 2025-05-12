using Microsoft.Extensions.DependencyInjection;
using Progress.Domain.Dtos.RabbitMqMessages;
using Progress.Domain.Entities;
using Progress.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sqids;
using Progress.Domain.RabbitMq;

namespace Progress.Infrastructure.RabbitMq.ConsumerLogic
{
    public class UserBoughtConsumer : IUserBoughtCourseConsumer
    {
        private readonly IServiceProvider _serviceProvider;

        public UserBoughtConsumer(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(string message)
        {
            var deserialize = JsonSerializer.Deserialize<UserBoughtCourseMessage>(message);

            using(var scope = _serviceProvider.CreateScope())
            {
                var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var sqids = scope.ServiceProvider.GetRequiredService<SqidsEncoder<long>>();
                var courseId = sqids.Decode(deserialize.courseId).Single();
                var userId = sqids.Decode(deserialize.userId).Single();

                var userProgress = new UserCourseProgress()
                {
                    CompletionPercentage = 0,
                    CourseId = courseId,
                    UserId = userId,
                    IsCompleted = false,
                    LastAccess = DateTime.UtcNow
                };

                await uof.genericRepository.Add<UserCourseProgress>(userProgress);
                await uof.Commit();
            }
        }
    }
}
