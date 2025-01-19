using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.Course
{
    public class ProjectDbContextFactory : IDesignTimeDbContextFactory<CourseDbContext>
    {
        public CourseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CourseDbContext>();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\Course.Api\appsettings.Development.json"), optional: false, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("sqlserver"));

            return new CourseDbContext(optionsBuilder.Options);
        }
    }
}
