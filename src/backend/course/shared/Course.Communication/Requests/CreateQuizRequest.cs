using Course.Exception;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateQuizRequest
    {
        [MinLength(1)]
        [MaxLength(256)]
        public string Title { get; set; }
        public string CourseId { get; set; }
        public string ModuleId { get; set; }
        public int PassingScore { get; set; }
    }
}
