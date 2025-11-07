using Course.Communication.Enums;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateQuestionRequest
    {
        public string QuestionText { get; set; }
        public decimal Points { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        public List<CreateAnswerOptionsRequest> AnswerOptions { get; set; }
    }
}
