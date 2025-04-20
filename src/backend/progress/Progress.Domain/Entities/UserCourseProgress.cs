using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Entities
{
    [Table("user_course_progress")]
    public class UserCourseProgress
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public decimal CompletionPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastAccess { get; set; }
    }
}
