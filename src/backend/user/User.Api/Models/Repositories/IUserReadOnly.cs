namespace User.Api.Models.Repositories
{
    public interface IUserReadOnly
    {
        public Task<bool> EmailExists(string email);
        public Task<UserModel?> UserByEmail(string email);
        public Task<UserModel?> UserByUid(Guid? uid);
        public Task<IList<UserModel>> GetUsersNotActive();
    }
}
