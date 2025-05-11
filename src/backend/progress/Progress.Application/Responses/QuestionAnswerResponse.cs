using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Responses
{
    public class QuestionAnswerResponse
    {
        public string QuestionId { get; set; }
        public bool isCorrect { get; set; }
    }
}
