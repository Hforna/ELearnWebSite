using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites.Quiz
{
    [Table("answer_options")]
    public class AnswerOption : BaseEntity
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
        public long QuestionId { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
