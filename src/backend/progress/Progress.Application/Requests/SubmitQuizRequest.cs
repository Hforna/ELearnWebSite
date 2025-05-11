using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Requests
{
    public class SubmitQuizRequest
    {
        public Guid AttemptId { get; set; }
        public List<SendQuestionRequest> Questions { get; set; }
    }
}
