using Azure.Messaging.ServiceBus;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Infrastructure.Data;
using Course.Infrastructure.Data.Course;
using Course.Infrastructure.Data.CourseD;
using Course.Infrastructure.Data.VideoD;
using Course.Infrastructure.Services.Azure;
using Course.Infrastructure.Services.Rest;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRepositories(services);
            AddServices(services);
            AddAzureStorage(services, configuration);
            AddServiceBus(services, configuration);
            AddRedis(services, configuration);
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var sqlconnection = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<CourseDbContext>(d => d.UseSqlServer(sqlconnection));

            services.AddScoped<VideoDbContext>(d => new VideoDbContext(configuration));
        }

        private static void AddRedis(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("redis");
            });
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICourseReadOnly, CourseRepository>();
            services.AddScoped<ICourseWriteOnly, CourseRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IModuleReadOnly, ModuleRepository>();
            services.AddScoped<IModuleWriteOnly, ModuleRepository>();
            services.AddScoped<ILessonReadOnly, LessonRepository>();
            services.AddScoped<ILessonWriteOnly, LessonRepository>();
            services.AddScoped<IVideoReadOnly, VideoRepository>();
            services.AddScoped<IVideoWriteOnly, VideoRepository>();
            services.AddScoped<IEnrollmentReadOnly, EnrollmentRepository>();
            services.AddScoped<IEnrollmentWriteOnly, EnrollmentRepository>();
        }

        private static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("services:azure:storage:blobClient");
            services.AddScoped<IStorageService>(d => new StorageService(new Azure.Storage.Blobs.BlobServiceClient(connectionString)));
        }

        private static void AddServiceBus(IServiceCollection service, IConfiguration configuration)
        {
            var serviceConnection = configuration.GetValue<string>("services:azure:serviceBus");

            var serviceBus = new ServiceBusClient(serviceConnection, new ServiceBusClientOptions {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            var sender = new DeleteCourseSender(serviceBus.CreateSender("delete"));

            service.AddScoped<IDeleteCourseSender>(d => sender);

            var processor = new DeleteCourseProcessor(serviceBus.CreateProcessor("delete", new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1
            }));

            service.AddSingleton(processor);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILinkService, LinkService>();
        }
    }
}
