using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites.Quiz
{
    [Table("questions")]
    public class QuestionEntity : BaseEntity
    {
        public long QuizId { get; set; }
        public QuizEntity Quiz { get; set; }
        public decimal Points { get; set; }
        public QuizTypeEnum Type { get; set; }
        public string QuestionText { get; set; }

    }
}
