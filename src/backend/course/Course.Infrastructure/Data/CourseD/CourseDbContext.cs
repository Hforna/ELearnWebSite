using Course.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.Course
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<CourseEntity> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CourseTopicsEntity> CourseTopics { get; set; }
        public DbSet<WishList> WishList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseEntity>().HasMany(d => d.Modules).WithOne(d => d.Course);
            modelBuilder.Entity<WishList>().HasOne(d => d.Course);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseDbContext).Assembly);
        }
    }
}
