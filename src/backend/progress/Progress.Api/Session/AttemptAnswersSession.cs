using Progress.Domain.Cache;
using Progress.Domain.Dtos;
using Progress.Domain.Exceptions;
using System.Text.Json;

namespace Progress.Api.Session
{
    public class AttemptAnswersSession : IAttemptAnswerSession
    {
        private readonly IHttpContextAccessor _httpContext;

        public AttemptAnswersSession(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public void AddAnswers(ShortQuizAnswersResponse shortQuiz, Guid attempt)
        {
            var session = _httpContext.HttpContext.Session;

            var response = JsonSerializer.Serialize(shortQuiz);
            session.SetString($"{attempt}", response);
        }

        public ShortQuizAnswersResponse GetAttemptQuestionAnswers(Guid attemptId)
        {
            var session = _httpContext.HttpContext.Session;

            var attempt = session.GetString($"{attemptId}");

            if (string.IsNullOrEmpty(attempt))
                throw new QuizAttemptException(ResourceExceptMessages.QUIZ_ATTEMPT_EXPIRED, System.Net.HttpStatusCode.NotFound);

            var deserialize = JsonSerializer.Deserialize<ShortQuizAnswersResponse>(attempt);

            session.Remove($"{attemptId}");

            return deserialize!;
        }
    }
}
