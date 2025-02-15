using X.PagedList;

namespace User.Api.Models.Repositories
{
    public interface IProfileReadOnly
    {
        public Task<ProfileModel?> ProfileByUserId(long id);
        public Task<IPagedList<ProfileModel>> GetUserProfiles(int page, int quantity);
    }
}
