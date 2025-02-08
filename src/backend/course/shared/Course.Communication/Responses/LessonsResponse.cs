using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class LessonsResponse
    {
        public string ModuleId { get; set; }
        public List<LessonResponse> Lessons { get; set; }
    }
}
