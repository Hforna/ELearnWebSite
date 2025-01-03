namespace User.Api.Models.Repositories
{
    public interface IUnitOfWork
    {
        public IUserReadOnly userReadOnly { get; set; }
        public IUserWriteOnly userWriteOnly { get; set; }

        public Task Commit();
    }
}
