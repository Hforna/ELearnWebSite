using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("modules")]
    public class Module : BaseEntity
    {
        [ForeignKey("Course")]
        public long CourseId { get; set; }
        public CourseEntity Course { get; set; }
        [MaxLength(100, ErrorMessage = "Name field length must be less than 100")]
        public string Name { get; set; }
        public int LessonsNumber { get; set; }
        public double Duration { get; set; }
    }
}
