using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Progress.Infrastructure.Data
{
    public class ProjectDbContext : DbContext
    {
        protected ProjectDbContext(DbContextOptions<ProjectDbContext> dbContext) : base(dbContext)
        {
        }

        //public DbSet<Certificate> Certificates { get; set; }
        //public DbSet<>
        //
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //
        //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectDbContext).Assembly);
        //}
    }
}
