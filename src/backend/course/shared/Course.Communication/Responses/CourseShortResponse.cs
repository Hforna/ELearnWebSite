using Course.Communication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class CourseShortResponse
    {
        public string CourseId { get; set; }
        public string TeacherId { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public LanguagesEnum CourseLanguage { get; set; }
        public CurrencyEnum CurrencyType { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
