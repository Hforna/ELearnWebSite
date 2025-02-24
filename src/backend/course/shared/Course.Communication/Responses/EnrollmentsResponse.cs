using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class EnrollmentsResponse
    {
        public int PageNumber { get; set; }
        public bool IsLastPage { get; set; }
        public bool IsFirstPage { get; set; }
        public int Count { get; set; }
        public List<CourseEnrollmentResponse> Enrollments { get; set; }
        public int TotalItemCount { get; set; }
    }
}
