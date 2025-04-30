using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites.Quiz
{
    [Table("quizzes")]
    public class QuizEntity : BaseEntity
    {
        public long CourseId { get; set; }
        public CourseEntity Course { get; set; }
        public string Title { get; set; }
        public int PassingScore { get; set; }
        public long ModuleId { get; set; }
        public List<QuestionEntity> Questions { get; set; } = [];
    }
}
