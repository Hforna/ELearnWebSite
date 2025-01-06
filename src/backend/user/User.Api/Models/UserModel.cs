using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Models
{
    [Table("users")]
    public class UserModel : IdentityUser<long>
    {
        public bool Active { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
