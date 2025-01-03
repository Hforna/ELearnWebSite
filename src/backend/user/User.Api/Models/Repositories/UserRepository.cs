using Microsoft.EntityFrameworkCore;
using User.Api.DbContext;

namespace User.Api.Models.Repositories
{
    public class UserRepository : IUserReadOnly, IUserWriteOnly
    {
        private UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext) => _dbContext = dbContext;

        public async Task CreateUser(UserModel user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<bool> EmailExists(string email)
        {
            var exists = await _dbContext.Users.FirstOrDefaultAsync(d =>  d.Email == email);

            return exists != null;
        }

        public async Task<UserModel?> UserByEmail(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(d => d.Email == email);
        }

        public async Task<UserModel?> UserByUid(Guid? uid)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(d => d.UserIdentifier == uid);
        }
    }
}
