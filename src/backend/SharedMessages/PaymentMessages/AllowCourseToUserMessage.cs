using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.PaymentMessages
{
    public class AllowCourseToUserMessage
    {
        public long UserId { get; set; }
        public List<long> CoursesIds { get; set; }
    }
}
