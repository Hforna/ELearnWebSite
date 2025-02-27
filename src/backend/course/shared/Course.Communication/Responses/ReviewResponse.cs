using Course.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class ReviewResponse
    {
        public string CourseId { get; set; }
        public string CustomerId { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public CourseRatingEnum Rating { get; set; }
    }
}
