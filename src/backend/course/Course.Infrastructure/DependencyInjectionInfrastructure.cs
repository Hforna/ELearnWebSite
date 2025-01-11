﻿using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Infrastructure.Data;
using Course.Infrastructure.Services.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var sqlconnection = configuration.GetConnectionString("sqlserver");
            services.AddDbContext<CourseDbContext>(d => d.UseSqlServer(sqlconnection));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICourseReadOnly, CourseRepository>();
            services.AddScoped<ICourseWriteOnly, CourseRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}
