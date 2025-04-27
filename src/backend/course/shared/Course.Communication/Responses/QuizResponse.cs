using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class QuizResponse
    {
        public string CourseId { get; set; }
        public string ModuleId { get; set; }
        public string Title { get; set; }
        public int PassingScore { get; set; } 
    }
}
