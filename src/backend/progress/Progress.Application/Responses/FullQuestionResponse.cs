using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Responses
{
    public class FullQuestionResponse
    {
        public string QuestionId { get; set; }
        public List<AnswersResponse> Answers { get; set; }
    }
}
