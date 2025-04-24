using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Progress.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            ConfigureRabbitMq(services, configuration);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<ProjectDbContext>(d => d.UseSqlServer(connection));
        }

        static void ConfigureRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var rabbitmqConnect = configuration.GetConnectionString("rabbitmq");
            var user = configuration.GetSection("services:rabbitmq:user").Value;
            var password = configuration.GetSection("services:rabbitmq:password").Value;

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseCircuitBreaker(d =>
                    {
                        d.ActiveThreshold = 20;
                        d.ResetInterval = TimeSpan.FromMinutes(30);
                        d.TripThreshold = 30;
                    });

                    cfg.UseMessageRetry(d => d.Interval(5, TimeSpan.FromSeconds(4)));

                    cfg.ConfigureEndpoints(context);
                    cfg.Host(new Uri(rabbitmqConnect), d =>
                    {
                        d.Username(user);
                        d.Password(password);
                    });
                });
            });
        }
    }
}
