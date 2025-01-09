using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("courses")]
    public class CourseEntity : BaseEntitie
    {
        public long TeacherId { get; set; }
        [MaxLength(100, ErrorMessage = "Title field length must be less than 100")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string TopicsCovered { get; set; }
        public double Price { get; set; }
    }
}
