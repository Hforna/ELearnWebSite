using Progress.Domain.Enums;

namespace Progress.Domain.Dtos
{
    public class QuestionResponse
    {
        public string id { get; set; }
        public string quizId { get; set; }
        string questionText { get; set; }
        public decimal points { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        public List<AnswerOptionsResponse> AnswerOptions { get; set; } = [];
    }
}