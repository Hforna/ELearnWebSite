using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMessages.CourseMessages
{
    public class CourseNoteMessage
    {
        public int CourseNumber { get; set; }
        public string UserId { get; set; }
        public decimal Note { get; set; }
    }
}
