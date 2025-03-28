using MassTransit;
using MercadoPago.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Repositories;
using Payment.Domain.Services.PaymentInterfaces;
using Payment.Domain.Services.RabbitMq;
using Payment.Domain.Services.Rest;
using Payment.Infrastructure.DataContext;
using Payment.Infrastructure.Services.PaymentAdapters;
using Payment.Infrastructure.Services.RabbitMq;
using Payment.Infrastructure.Services.Rest;
using SharedMessages.PaymentMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure
{
    public static class DependencyInjectionInfrastructure
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRestService(services, configuration);
            AddRepositories(services);
            AddPaymentServices(services, configuration);
            RabbitMq(services, configuration);
        }

        static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionSql = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<PaymentDbContext>(d => d.UseSqlServer(connectionSql));
        }

        static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IOrderReadOnly, OrderRepository>();
            services.AddScoped<IOrderWriteOnly, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionReadOnly, TransactionRepository>();
            services.AddScoped<ITransactionWriteOnly, TransactionRepository>();
            services.AddScoped<IPaymentReadOnly, PaymentRepository>();
            services.AddScoped<IPaymentWriteOnly, PaymentRepository>();
        }

        static void RabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqServer = configuration.GetConnectionString("rabbitmq");
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                    cfg.Host(new Uri(rabbitMqServer), f =>
                    {
                        f.Username("guest");
                        f.Password("guest");
                    });
                    cfg.Message<AllowCourseToUserMessage>(d => d.SetEntityName("payment_exchange"));
                    cfg.Publish<AllowCourseToUserMessage>(d => d.ExchangeType = "direct");
                });
            });
        }

        static void AddProducers(IServiceCollection services)
        {
            services.AddScoped<ICourseProducerService, CourseProducerService>();
        }

        static void AddPaymentServices(IServiceCollection services, IConfiguration configuration)
        {

            MercadoPagoConfig.AccessToken = configuration.GetValue<string>("apis:mercadoPago:accessToken");

            //services.AddScoped<IPixGatewayService, MercadoPagoGatewayAdapter>();

            var trioClientId = configuration.GetValue<string>("apis:trio:clientId"); 
            var trioClientSecret = configuration.GetValue<string>("apis:trio:clientSecret");

            services.AddScoped<IPixGatewayService>(d => new TrioPixGatewayAdapter(services.BuildServiceProvider()
                .CreateScope().ServiceProvider.GetRequiredService<IHttpClientFactory>(), 
                trioClientId!, trioClientSecret!));
        }

        static void AddRestService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRestService, CourseRestService>();
            services.AddScoped<IUserRestService, UserRestService>();
            services.AddScoped<ILocationRestService, GeoLocationRestService>();
            services.AddScoped<ICurrencyExchangeService>(d => new CurrencyFreaksExchangeRestService(
                configuration.GetValue<string>("apis:currencyFreaks:apiKey"), 
                services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IHttpClientFactory>()));
        }
    }
}
