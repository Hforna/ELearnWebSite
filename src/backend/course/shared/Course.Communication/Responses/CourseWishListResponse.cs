using Course.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class CourseWishListResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CourseId { get; set; }
    }
}
