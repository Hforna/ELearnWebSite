using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("courseTopics")]
    public class CourseTopicsEntity : BaseEntity
    {
        public long CourseId { get; set; }
        public string? Topic { get; set; }
    }
}
