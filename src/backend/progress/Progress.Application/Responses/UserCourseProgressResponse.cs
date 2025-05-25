using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Application.Responses
{
    public class UserCourseProgressResponse
    {
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public decimal CompletionPercentage { get; set; }
        public int TotalLessonsCompleted { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastAccess { get; set; }
    }
}
