using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progress.Domain.Dtos;

namespace Progress.Application.Responses
{
    public class QuizAttemptResponse
    {
        public Guid Id { get; set; }
        public string QuizId { get; set; }
        public DateTime StartedAt { get; set; }
        public QuizDto Quiz { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
