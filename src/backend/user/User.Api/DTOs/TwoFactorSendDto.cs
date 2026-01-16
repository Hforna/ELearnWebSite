namespace User.Api.DTOs
{
    public class TwoFactorSendDto
    {
        public string email { get; set; }
        public string password { get; set; }
        public string method { get; set; }
    }
}
