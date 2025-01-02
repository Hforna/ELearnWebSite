using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Models
{
    [Table("roles")]
    public class RoleModel : IdentityRole<long>
    {
        public RoleModel(string name) => Name = name;
    }
}
