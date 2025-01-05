
using User.Api.DbContext;

namespace User.Api.Models.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _dbContext;
        public IUserReadOnly userReadOnly { get; set; }
        public IUserWriteOnly userWriteOnly { get; set; }
        public IProfileReadOnly profileReadOnly { get; set; }
        public IProfileWriteOnly profileWriteOnly { get; set; }

        public UnitOfWork(UserDbContext dbContext, IUserReadOnly userReadOnly, 
            IUserWriteOnly userWriteOnly, IProfileReadOnly profileReadOnly, IProfileWriteOnly profileWriteOnly)
        {
            _dbContext = dbContext;
            this.userReadOnly = userReadOnly;
            this.userWriteOnly = userWriteOnly;
            this.profileReadOnly = profileReadOnly;
            this.profileWriteOnly = profileWriteOnly;
        }

        public async Task Commit() => await _dbContext.SaveChangesAsync();
    }
}
