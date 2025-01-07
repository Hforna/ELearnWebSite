

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
                new RoleModel("admin"),
                new RoleModel("customer"),
                new RoleModel("teacher")
            );

            base.OnModelCreating(builder);
        }
    }
}
