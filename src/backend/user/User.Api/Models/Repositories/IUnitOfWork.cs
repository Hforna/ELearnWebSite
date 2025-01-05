namespace User.Api.Models.Repositories
{
    public interface IUnitOfWork
    {
        public IUserReadOnly userReadOnly { get; set; }
        public IUserWriteOnly userWriteOnly { get; set; }
        public IProfileReadOnly profileReadOnly { get; set; }
        public IProfileWriteOnly profileWriteOnly { get; set; }

        public Task Commit();
    }
}
