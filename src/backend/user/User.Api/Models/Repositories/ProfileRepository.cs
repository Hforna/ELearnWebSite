using Microsoft.EntityFrameworkCore;
using User.Api.DbContext;

namespace User.Api.Models.Repositories
{
    public class ProfileRepository : IProfileReadOnly, IProfileWriteOnly
    {
        private readonly UserDbContext _dbContext;

        public ProfileRepository(UserDbContext dbContext) => _dbContext = dbContext;

        public async Task AddProfile(ProfileModel profile) => await _dbContext.Profiles.AddAsync(profile);

        public async Task<ProfileModel?> ProfileByUserId(long id)
        {
            return await _dbContext.Profiles.SingleOrDefaultAsync(d => d.UserId == id);
        }
    }
}
