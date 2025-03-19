using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Rest;
using Payment.Infrastructure.DataContext;
using Payment.Infrastructure.Services.Rest;
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
        }

        static void AddRestService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICourseRestService, CourseRestService>();
            services.AddScoped<IUserRestService, UserRestService>();
        }
    }
}
