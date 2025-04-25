using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites.Quiz
{
    [Table("correct_answers")]
    public class CorrectAnswer : BaseEntity
    {
        public string CorrectText { get; set; }
        public long QuestionId { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
