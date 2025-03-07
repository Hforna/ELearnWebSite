namespace User.Api.Models.Repositories
{
    public interface IProfileWriteOnly
    {
        public Task AddProfile(ProfileModel profile);
        public void UpdateProfile(ProfileModel profile);
    }
}
