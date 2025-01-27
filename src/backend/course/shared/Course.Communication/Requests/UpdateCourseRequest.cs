using Course.Communication.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class UpdateCourseRequest
    {
        public string Title { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public string Description { get; set; }
        public IList<CreateCourseTopicsRequest> TopicsCovered { get; set; }
        public double Price { get; set; }
        public LanguagesEnum CourseLanguage { get; set; }
    }
}
