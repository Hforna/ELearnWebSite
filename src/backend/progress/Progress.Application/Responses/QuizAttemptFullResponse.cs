using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Responses
{
    public class QuizAttemptFullResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string QuizId { get; set; }
        public DateTime AttemptedAt { get; set; }
        public decimal Score { get; set; }
        public List<QuestionAnswerResponse> QuestionAnswers { get; set; }
        public bool Passed { get; set; }
    }
}
