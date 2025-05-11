using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Requests
{
    public class SendQuestionRequest
    {
        public string QuestionId { get; set; }
        public string AnswerId { get; set; }
    }
}
