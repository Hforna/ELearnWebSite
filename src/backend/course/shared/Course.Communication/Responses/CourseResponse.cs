using Course.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class CourseResponse : BaseResponse
    {
        public string Id { get; set; }
        public string TeacherId { get; set; }
        public string TeacherProfile { get; set; }
        public int? Enrollments { get; set; }
        public double? Duration { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public CurrencyEnum CurrencyType { get; set; }
        public LanguagesEnum CourseLanguage { get; set; }
        public string ThumbnailUrl { get; set; }
        public IList<CourseTopicsResponse> CourseTopics { get; set; }
    }
}
