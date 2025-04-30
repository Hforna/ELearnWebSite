using Course.Communication.Enums;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class QuestionResponse
    {
        public string Id { get; set; }
        public string QuizId { get; set; }
        string QuestionText { get; set; }
        public decimal Points { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        public List<AnswerOptionsResponse> AnswerOptions { get; set; } = [];
    }
}
