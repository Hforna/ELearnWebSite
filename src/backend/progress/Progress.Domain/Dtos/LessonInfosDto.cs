using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public class LessonInfosDto
    {
        public string Id { get; set; }
        public string ModuleId { get; set; }
        public string CourseId { get; set; }
        public double Duration { get; set; }
    }
}
