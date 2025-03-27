using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class ProfileDto
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string fullName { get; set; }
    }
}
