using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Responses
{
    public class QuizSubmitResponse
    {
        public Guid AttemptId { get; set; }
        public string QuizId { get; set; }
        public bool Passed { get; set; }
        public List<QuestionAnswerResponse> QuestionAnswers { get; set; }
        public float TotalPoints { get; set; }
    }
}
