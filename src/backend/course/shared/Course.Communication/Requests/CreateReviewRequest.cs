using Course.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateReviewRequest
    {
        public string Text { get; set; }
        public CourseRatingEnum Note { get; set; }
    }
}
