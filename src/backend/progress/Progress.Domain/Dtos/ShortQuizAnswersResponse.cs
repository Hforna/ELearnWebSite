using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos
{
    public class ShortQuizResponse
    {
        public string QuizId { get; set; }
        public List<QuestionAnswerDto> AnswerDtos { get; set; }
        public decimal PassingScore { get; set; }
    }
}
