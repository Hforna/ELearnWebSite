using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class Enrollment : BaseEntity
    {
        public long CustomerId { get; set; }
        public long CourseId { get; set; }
        public CourseEntity Course { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
