namespace User.Api.Models.Repositories
{
    public interface IUserWriteOnly
    {
        public Task CreateUser(UserModel user);
    }
}
