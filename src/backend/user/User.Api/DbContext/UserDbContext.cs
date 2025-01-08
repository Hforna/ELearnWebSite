

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using User.Api.Models;

namespace User.Api.DbContext
{
    public class UserDbContext : IdentityDbContext<UserModel, RoleModel, long>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<ProfileModel> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RoleModel>().HasData(
                new RoleModel() { Id = 1, Name = "admin", NormalizedName = "ADMIN"},
                new RoleModel() { Id = 2, Name = "customer", NormalizedName = "CUSTOMER"},
                new RoleModel() { Id = 3, Name = "teacher", NormalizedName = "TEACHER"}
            );

            base.OnModelCreating(builder);
        }
    }
}
