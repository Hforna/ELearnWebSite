using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class AnswerOptionsResponse
    {
        public string Id { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionId { get; set; }
    }
}
