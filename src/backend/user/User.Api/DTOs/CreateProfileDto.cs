using User.Api.Models;

namespace User.Api.DTOs
{
    public class CreateProfileDto
    {
        public IFormFile ProfilePicture { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public string FullName { get; set; }
    }
}
