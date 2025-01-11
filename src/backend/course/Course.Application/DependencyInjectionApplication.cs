using AutoMapper;
using Course.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application
{
    public static class DependencyInjectionApplication
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddSqids(services, configuration);
            ConfigureAutoMapper(services);
        }

        private static void AddSqids(IServiceCollection services, IConfiguration configuration)
        {
            var minLength = configuration.GetValue<int>("services:sqids:minLength");
            var alphabet = configuration.GetSection("services:sqids:alphabet").Value;

            services.AddSingleton(d => new SqidsEncoder<long>(new SqidsOptions() { Alphabet = alphabet, MinLength = minLength}));
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddScoped(mapper =>
            {
                var config = new MapperConfiguration(d => d.AddProfile(new MapperService()));

                return config.CreateMapper();
            });
        }
    }
}
