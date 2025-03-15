using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class ReviewResponseEntity : BaseEntity
    {
        public long ReviewId { get; set; }
        public long CourseId { get; set; }
        public string Text { get; set; }
        public long TeacherId { get; set; }
    }
}
