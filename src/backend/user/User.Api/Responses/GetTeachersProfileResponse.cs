namespace User.Api.Responses
{
    public class GetTeachersProfileResponse
    {
        public int PageNumber { get; set; }
        public bool IsLastPage { get; set; }
        public bool IsFirstPage { get; set; }
        public int Count { get; set; }
        public List<ProfileShortResponse> Profiles { get; set; }
        public int TotalItemCount { get; set; }
    }
}
