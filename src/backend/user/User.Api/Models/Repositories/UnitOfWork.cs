
using User.Api.DbContext;

namespace User.Api.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _dbContext;
        public IUserReadOnly userReadOnly { get; set; }
        public IUserWriteOnly userWriteOnly { get; set; }

        public UnitOfWork(UserDbContext dbContext, IUserReadOnly userReadOnly, IUserWriteOnly userWriteOnly)
        {
            _dbContext = dbContext;
            this.userReadOnly = userReadOnly;
            this.userWriteOnly = userWriteOnly;
        }

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
