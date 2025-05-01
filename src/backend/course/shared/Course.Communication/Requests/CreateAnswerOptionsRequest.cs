using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateAnswerOptionsRequest
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
