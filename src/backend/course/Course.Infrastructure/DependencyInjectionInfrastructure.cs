using Azure.Messaging.ServiceBus;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Infrastructure.Data;
using Course.Infrastructure.Data.Course;
using Course.Infrastructure.Data.CourseD;
using Course.Infrastructure.Data.VideoD;
using Course.Infrastructure.Services.Azure;
using Course.Infrastructure.Services.RabbitMq;
using Course.Infrastructure.Services.Rest;
using MassTransit;
using MassTransit.Transports.Fabric;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SharedMessages.CourseMessages;
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
            AddServices(services, configuration);
            AddAzureStorage(services, configuration);
            AddServiceBus(services, configuration);
            AddRedis(services, configuration);
            AddRabbitMq(services, configuration);
            AddMessagingServices(services);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var sqlconnection = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<CourseDbContext>(d => d.UseSqlServer(sqlconnection));

            services.AddScoped<VideoDbContext>(d => new VideoDbContext(configuration));
        }

        static void AddRedis(IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("redis");
            });
        }

        static void AddRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var rabbitmqServer = configuration.GetConnectionString("rabbitmq");
            services.AddMassTransit((x) =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host(new Uri(rabbitmqServer!), f =>
                    {
                        f.Username("guest");
                        f.Password("guest");
                    });
                    cfg.Message<CourseNoteMessage>(x => x.SetEntityName("course-exchange"));
                    cfg.Publish<CourseNoteMessage>(x => x.ExchangeType = "direct");

                    cfg.Message<CourseCreatedMessage>(x => x.SetEntityName("course-exchange"));
                    cfg.Publish<CourseCreatedMessage>(x => x.ExchangeType = "direct");
                });
            });
        }

        static void AddMessagingServices(IServiceCollection services)
        {
            services.AddScoped<IUserSenderService, UserProducerService>();
        }

        static void AddRepositories(IServiceCollection services)
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
            services.AddScoped<IReviewReadOnly, ReviewRepository>();
            services.AddScoped<IReviewWriteOnly, ReviewRepository>();
            services.AddScoped<IWishListReadOnly, WishListRepository>();
            services.AddScoped<IWishListWriteOnly, WishListRepository>();
        }

        static void AddAzureStorage(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("services:azure:storage:blobClient");
            services.AddScoped<IStorageService>(d => new StorageService(new Azure.Storage.Blobs.BlobServiceClient(connectionString)));
        }

        static void AddServiceBus(IServiceCollection service, IConfiguration configuration)
        {
            var serviceConnection = configuration.GetValue<string>("services:azure:serviceBus");

            var serviceBus = new ServiceBusClient(serviceConnection, new ServiceBusClientOptions {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });

            var sender = new DeleteCourseSender(serviceBus.CreateSender("delete"));

            var processor = new DeleteCourseProcessor(serviceBus.CreateProcessor("delete", new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1
            }));

            var moduleSender = new NewModuleSender(serviceBus.CreateSender("newModule"));

            var moduleProcessor = new NewModuleProcessor(serviceBus.CreateProcessor("newModule", new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1
            }));

            service.AddScoped<INewModuleSender>(d => moduleSender);
            service.AddScoped<IDeleteCourseSender>(d => sender);

            service.AddSingleton(processor);
            service.AddSingleton(moduleProcessor);
        }

        static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILinkService, LinkService>();
            services.AddScoped<ILocationService, GeoLocationService>();
            services.AddScoped<ICurrencyExchangeService, CurrencyFreaksService>(d => new CurrencyFreaksService(
                services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IHttpClientFactory>(),
                configuration.GetValue<string>("apis:currencyFreaks:apiKey")!));
        }
    }
}
