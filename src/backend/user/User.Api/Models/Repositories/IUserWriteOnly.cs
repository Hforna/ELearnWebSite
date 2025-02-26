namespace User.Api.Models.Repositories
{
    public interface IUserWriteOnly
    {
        public Task CreateUser(UserModel user);
        public void DeleteUser(UserModel user);
        public void UpdateUser(UserModel user);
    }
}
