using Progress.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Entities
{
    [Table("quiz_attempts")]
    public class QuizAttempts
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long QuizId { get; set; }
        public long CourseId { get; set; }
        public decimal Score { get; set; } = 0;
        public QuizAttemptStatusEnum Status { get; set; }
        public bool Passed { get; set; } = false;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime AttemptedAt { get; set; }
        [InverseProperty("Attempt")]
        public IList<UserQuizResponse> QuizResponses { get; set; }
    }
}
