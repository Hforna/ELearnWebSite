﻿using AutoMapper;
using Course.Application.Services;
using Course.Application.Services.AutoMapper;
using Course.Application.UseCases.Course;
using Course.Application.UseCases.Courses;
using Course.Application.UseCases.Enrollments;
using Course.Application.UseCases.Lessons;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Quizzes;
using Course.Application.UseCases.Repositories.Course;
using Course.Application.UseCases.Repositories.Enrollments;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Application.UseCases.Repositories.Modules;
using Course.Application.UseCases.Repositories.Quizzes;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Application.UseCases.Repositories.WishLists;
using Course.Application.UseCases.Reviews;
using Course.Application.UseCases.WishLists;
using Course.Domain.Repositories;
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
            services.AddScoped<ICreateModule, CreateModule>();
            services.AddScoped<IGetCourses, GetCourses>();
            services.AddScoped<IGetCourse, GetCourse>();
            services.AddScoped<IDeleteCourse, DeleteCourse>();
            services.AddScoped<IDeleteModule, DeleteModule>();
            services.AddScoped<IUpdateCourse, UpdateCourse>();
            services.AddScoped<ICreateLesson, CreateLesson>();
            services.AddScoped<IGetModules, GetModules>();
            services.AddScoped<IGetLessons, GetLessons>();
            services.AddScoped<IGetLesson, GetLesson>();
            services.AddScoped<IChangeModulePosition, ChangeModulePosition>();
            services.AddScoped<IUpdateModule, UpdateModule>();
            services.AddScoped<IDeleteLesson, DeleteLesson>();
            services.AddScoped<IUpdateLesson, UpdateLesson>();
            services.AddScoped<IGetTenMostPopularWeekCourses, GetTenMostPopularWeekCourses>();
            services.AddScoped<IGetCourseEnrollments, GetCourseEnrollments>();
            services.AddScoped<ICreateReview, CreateReview>();
            services.AddScoped<IDeleteReview, DeleteReview>();
            services.AddScoped<ITeacherCourses, TeacherCourses>();
            services.AddScoped<ICourseThatUserBought, CoursesThatUserBought>();
            services.AddScoped<IAddItemToWishList, AddItemToWishList>();
            services.AddScoped<IRemoveCourseFromWishList, RemoveCourseFromWishList>();
            services.AddScoped<IAnswerReview, AnswerReview>();
            services.AddScoped<IGetReviews, GetReviews>();
            services.AddScoped<IGetReview, GetReview>();
            services.AddScoped<IGetUserWishList, GetUserWishList>();
            services.AddScoped<ICreateQuiz, CreateQuiz>();
            services.AddScoped<IGetQuizById, GetQuizById>();
            services.AddScoped<ICreateQuestion, CreateQuestion>();
            services.AddScoped<IDeleteQuestion, DeleteQuestion>();
            services.AddScoped<IDeleteQuiz, DeleteQuiz>();
            services.AddScoped<IUserGotCourse, UserGotCourse>();
            services.AddScoped<ICourseLessonsCount, CourseLessonsCount>();
            services.AddScoped<IGetLessonShortInfos, GetLessonShortInfos>();

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
