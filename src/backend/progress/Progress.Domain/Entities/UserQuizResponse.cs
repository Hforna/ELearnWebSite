using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Entities
{
    [Table("user_quiz_responses")]
    internal class UserQuizResponse
    {
        public Guid Id { get; set; }
        public Guid AttemptId { get; set; }
        public long QuestionId { get; set; }
        public long SelectedOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}
