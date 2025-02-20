using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class CoursesResponse
    {
        public int PageNumber { get; set; }
        public bool IsLastPage { get; set; }
        public bool IsFirstPage { get; set; }
        public int Count { get; set; }
        public List<CourseShortResponse> Courses { get; set; }
        public int TotalItemCount { get; set; }
    }
}
