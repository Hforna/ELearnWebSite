using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public sealed record UserInfosDto
    {
        public string id { get; set; }
        public string userName { get; set; }
        public string email { get; set; }
        public string? phoneNumber { get; set; }
        public bool is2fa { get; set; }
    }
}
