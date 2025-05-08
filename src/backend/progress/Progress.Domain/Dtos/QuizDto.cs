using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public class QuizDto
    {
        public string id { get; set; }
        public string courseId { get; set; }
        public string moduleId { get; set; }
        public string title { get; set; }
        public int passingScore { get; set; }
        public int questionsNumber { get; set; } = 0;
        public List<QuestionResponse> Questions { get; set; } = [];
        public QuizMetadataDto Metadata { get; set; }
    }
}
