using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Models
{
    [Table("profiles")]
    public class ProfileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long UserId { get; set; }
        public UserModel User { get; set; }
        public string ProfilePicture { get; set; }
    }
}
