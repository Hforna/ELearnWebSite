namespace User.Api.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool Is2fa { get; set; }
        public string Cpf { get; set; }
    }
}
