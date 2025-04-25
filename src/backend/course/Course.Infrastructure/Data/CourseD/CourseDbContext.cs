using Course.Domain.Entitites;
using Course.Domain.Entitites.Quiz;
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
        public DbSet<ReviewResponseEntity> ReviewResponse { get; set; }
        public DbSet<QuizEntity> Quizzes { get; set; }
        public DbSet<QuestionEntity> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<CorrectAnswer> CorrectAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CourseEntity>().HasMany(d => d.Modules).WithOne(d => d.Course);
            modelBuilder.Entity<WishList>().HasOne(d => d.Course);

            modelBuilder.Entity<QuizEntity>().HasMany(d => d.Questions).WithOne(d => d.Quiz);
            modelBuilder.Entity<QuizEntity>().HasOne(d => d.Course).WithMany().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<QuestionEntity>().HasOne(d => d.Quiz).WithMany(d => d.Questions).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AnswerOption>().HasOne(d => d.Question);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseDbContext).Assembly);
        }
    }
}
