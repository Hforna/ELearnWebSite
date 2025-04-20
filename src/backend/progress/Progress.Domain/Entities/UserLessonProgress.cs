using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Entities
{
    [Table("user_lesson_progress")]
    public class UserLessonProgress
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public long LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
