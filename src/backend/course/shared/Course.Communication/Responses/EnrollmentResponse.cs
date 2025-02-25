using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class EnrollmentResponse
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public string CourseId { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
