namespace Progress.Domain.Dtos
{
    public class AnswerOptionsResponse
    {
        public string id { get; set; }
        public bool isCorrect { get; set; }
        public string questionId { get; set; }
    }
}