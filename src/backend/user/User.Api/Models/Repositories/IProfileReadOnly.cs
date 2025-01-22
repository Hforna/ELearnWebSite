namespace User.Api.Models.Repositories
{
    public interface IProfileReadOnly
    {
        public Task<ProfileModel?> ProfileByUserId(long id);
    }
}
