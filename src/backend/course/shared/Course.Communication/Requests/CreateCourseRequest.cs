using Course.Communication.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public record CreateCourseRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? ThumbnailImage { get; set; }
        public LanguagesEnum CourseLanguage { get; set; }
        public double Price { get; set; }
        public IList<CreateCourseTopicsRequest> CourseTopics { get; set; }
    }
}
