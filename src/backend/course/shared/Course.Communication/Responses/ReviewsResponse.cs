using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class ReviewsResponse
    {
        public string CourseId { get; set; }
        public IList<ReviewResponse> Reviews { get; set; }
    }
}
