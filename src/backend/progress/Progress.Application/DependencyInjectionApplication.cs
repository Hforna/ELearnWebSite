using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Progress.Application.services;
using Progress.Application.UseCases.Interfaces;
using Progress.Application.UseCases.QuizAttempt;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application
{
    public static class DependencyInjectionApplication
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddSqids(services, configuration);
            ConfigureAutoMapper(services);
            AddRepositores(services);
        }

        static void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(d =>
            {
                d.AddProfile(new AutoMapperConfiguration());
            });
        }

        static void AddRepositores(IServiceCollection services)
        {
            services.AddScoped<IStartQuizAttempt, StartQuizAttempt>();
            services.AddScoped<ISubmitQuizAttempt, SubmitQuizAttempt>();
        }

        static void AddSqids(IServiceCollection services, IConfiguration configuration)
        {
            var minLength = configuration.GetSection("services:sqids:minLength").Value;
            var alphabet = configuration.GetSection("services:sqids:alphabet").Value;

            services.AddSingleton(new SqidsEncoder<long>(new SqidsOptions()
            {
                Alphabet = alphabet,
                MinLength = int.Parse(minLength!)
            }));
        }
    }
}
