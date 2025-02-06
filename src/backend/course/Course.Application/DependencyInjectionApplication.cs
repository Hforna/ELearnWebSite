using AutoMapper;
using Course.Application.Services;
using Course.Application.Services.AutoMapper;
using Course.Application.UseCases.Course;
using Course.Application.UseCases.Lessons;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Course;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Application.UseCases.Repositories.Modules;
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
            AddRepositories(services);
            AddEmailSerice(services, configuration);
        }

        private static void AddSqids(IServiceCollection services, IConfiguration configuration)
        {
            var minLength = configuration.GetValue<int>("services:sqids:minLength");
            var alphabet = configuration.GetSection("services:sqids:alphabet").Value;

            services.AddSingleton(d => new SqidsEncoder<long>(new SqidsOptions() { Alphabet = alphabet, MinLength = minLength}));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICreateCourse, CreateCourse>();
            services.AddScoped<CreateModule, CreateModule>();
            services.AddScoped<IGetCourses, GetCourses>();
            services.AddScoped<IGetCourse, GetCourse>();
            services.AddScoped<IDeleteCourse, DeleteCourse>();
            services.AddScoped<IDeleteModule, DeleteModule>();
            services.AddScoped<IUpdateCourse, UpdateCourse>();
            services.AddScoped<ICreateLesson, CreateLesson>();

            services.AddSingleton(new FileService());
        }

        private static void AddEmailSerice(IServiceCollection services, IConfiguration configuration)
        {
            var userName = configuration.GetValue<string>("services:email:userName");
            var password = configuration.GetValue<string>("services:email:password");
            var email = configuration.GetValue<string>("services:email:email");
            services.AddSingleton(new EmailService(userName, email, password));
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            services.AddScoped(mapper =>
            {
                var sqid = mapper.GetRequiredService<SqidsEncoder<long>>();
                var config = new MapperConfiguration(d => d.AddProfile(new MapperService(sqid)));

                return config.CreateMapper();
            });
        }
    }
}
