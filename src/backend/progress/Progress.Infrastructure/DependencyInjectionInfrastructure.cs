using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Progress.Domain.RabbitMq;
using Progress.Domain.RabbitMq.Publisher;
using Progress.Domain.Repositories;
using Progress.Domain.Rest;
using Progress.Infrastructure.Data;
using Progress.Infrastructure.RabbitMq.BusinessLogic;
using Progress.Infrastructure.RabbitMq.ConsumerLogic;
using Progress.Infrastructure.RabbitMq.Publish;
using Progress.Infrastructure.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Progress.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static async Task AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            await AddRabbitMq(services, configuration);
            AddRestService(services);
            AddRedisCache(services, configuration);
            AddRepositories(services);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<ProjectDbContext>(d => d.UseSqlServer(connection));
        }

        static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository, GenericRepository>();
            services.AddScoped<IQuizAttemptReadOnly, QuizAttemptRepository>();
            services.AddScoped<IQuizAttemptWriteOnly, QuizAttemptRepository>();
            services.AddScoped<IUserCourseProgressReadOnly, UserCourseProgressRepository>();
            services.AddScoped<IUserCourseProgressWriteOnly, UserCourseProgressRepository>();
            services.AddScoped<IUserLessonProgressReadOnly, UserLessonProgressRepository>();
            services.AddScoped<IUserLessonProgressWriteOnly, UserLessonProgressRepository>();
        }

        static async Task AddRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var connection = await new ConnectionFactory()
            {
                Port = configuration.GetValue<int>("services:rabbitMq:port"),
                HostName = configuration.GetValue<string>("services:rabbitMq:hostName")!,
                UserName = configuration.GetValue<string>("services:rabbitMq:username"),
                Password = configuration.GetValue<string>("services:rabbitMq:password"),
            }.CreateConnectionAsync();
            services.AddSingleton<IConnection>(connection);
            
            services.AddScoped<IUserSubmitQuizPublisher, UserSubmitQuizPublisher>();

            services.AddSingleton<IUserDeletedConsumer, UserDeletedConsumer>();
            services.AddSingleton<IUserBoughtCourseConsumer, UserBoughtConsumer>();
            services.AddSingleton<IUserSubmitQuizConsumer, UserSubmitQuizConsumer>();
        }

        static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("redis");

            services.AddStackExchangeRedisCache(redisOpt =>
            {
                redisOpt.Configuration = connection;
            });
        }

        static void AddRestService(IServiceCollection services)
        {
            services.AddScoped<ICourseRestService, CourseRestService>();
            services.AddScoped<IUserRestService, UserRestService>();
        }
    }
}
