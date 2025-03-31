using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.ApplicationServices;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Services;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application
{
    public static class DependencyInjectionApplication
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddSqids(services, configuration);
            AddAutoMapper(services);
            AddRepositories(services);
        }

        static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IWebhookService, WebhookService>();
        }

        static void AddSqids(IServiceCollection services, IConfiguration configuration)
        {
            int minLength = int.Parse(configuration.GetSection("services:sqids:minLength").Value!);
            string alphabet = configuration.GetSection("services:sqids:alphabet").Value!;

            services.AddSingleton<SqidsEncoder<long>>(d => new SqidsEncoder<long>(new SqidsOptions()
            {
                Alphabet = alphabet,
                MinLength = minLength
            }));
        }

        static void AddAutoMapper(IServiceCollection services)
        {
            services.AddScoped(mapper =>
            {
                var sqids = mapper.GetRequiredService<SqidsEncoder<long>>();
                var config = new MapperConfiguration(d => d.AddProfile(new MapperService(sqids)));

                return config.CreateMapper();
            });
        }
    }
}
