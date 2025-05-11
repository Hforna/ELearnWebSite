using Progress.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Cache
{
    public interface IAttemptAnswerSession
    {
        public ShortQuizAnswersResponse GetAttemptQuestionAnswers(Guid attemptId);
    }
}
