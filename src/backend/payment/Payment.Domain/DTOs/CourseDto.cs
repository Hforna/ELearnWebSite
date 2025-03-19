using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public record CourseDto
    {
        public string id { get; set; }
        public string teacherId { get; set; }
        public string teacherProfile { get; set; }
        public int? enrollments { get; set; }
        public double? duration { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public double price { get; set; }
    }
}
