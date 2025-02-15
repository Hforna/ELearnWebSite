using Microsoft.EntityFrameworkCore;
using User.Api.DbContext;
using X.PagedList;
using X.PagedList.Extensions;

namespace User.Api.Models.Repositories
{
    public class ProfileRepository : IProfileReadOnly, IProfileWriteOnly
    {
        private readonly UserDbContext _dbContext;

        public ProfileRepository(UserDbContext dbContext) => _dbContext = dbContext;

        public async Task AddProfile(ProfileModel profile) => await _dbContext.Profiles.AddAsync(profile);

        public async Task<IPagedList<ProfileModel>> GetUserProfiles(int page, int quantity)
        {
            var roleTeacher = await _dbContext.Roles.FirstOrDefaultAsync(d => d.Name == "teacher");
            var usersTeacher = _dbContext.UserRoles.Where(d => d.RoleId == roleTeacher.Id).Select(d => d.UserId);

            var query = _dbContext.Profiles.Where(d => usersTeacher.Contains(d.UserId));

            return query.ToPagedList(page, quantity);
        }

        public async Task<ProfileModel?> ProfileByUserId(long id)
        {
            return await _dbContext.Profiles.SingleOrDefaultAsync(d => d.UserId == id);
        }
    }
}
