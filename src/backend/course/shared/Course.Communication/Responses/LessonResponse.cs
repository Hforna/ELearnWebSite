using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class LessonResponse
    {
        public string ModuleId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public int TimeInMinutes { get; set; }
        public int Order { get; set; }
        public bool isFree { get; set; }
    }
}
