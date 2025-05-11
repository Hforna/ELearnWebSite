using Progress.Domain.Cache;
using Progress.Domain.Dtos;
using Progress.Domain.Exceptions;
using System.Text.Json;

namespace Progress.Api.Session
{
    public class AttemptAnswersSession : IAttemptAnswerSession
    {
        private readonly ISession _session;

        public AttemptAnswersSession(ISession session)
        {
            _session = session;
        } 

        public ShortQuizAnswersResponse GetAttemptQuestionAnswers(Guid attemptId)
        {
            var attempt = _session.GetString($"{attemptId}");

            if (string.IsNullOrEmpty(attempt))
                throw new QuizAttemptException(ResourceExceptMessages.QUIZ_ATTEMPT_EXPIRED, System.Net.HttpStatusCode.NotFound);

            var deserialize = JsonSerializer.Deserialize<ShortQuizAnswersResponse>(attempt);

            _session.Remove($"{attemptId}");

            return deserialize!;
        }
    }
}
