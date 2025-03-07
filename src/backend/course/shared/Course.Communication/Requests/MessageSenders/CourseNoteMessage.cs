using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests.MessageSenders
{
    public class SendCourseNoteMessage
    {
        public int CourseNumber { get; set; }
        public string UserId { get; set; }
        public decimal Note { get; set; }
    }
}
