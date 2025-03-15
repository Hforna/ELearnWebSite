using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class ReviewAnswerResponse : BaseResponse
    {
        public string TeacherId { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
        public string ReviewId { get; set; }
        public string CourseId { get; set; }
    }
}
