using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.DTOs
{
    public class GetCoursesFilterDto
    {
        public string? Text { get; set; }
        public List<LanguagesEnum>? Languages { get; set; }
        public List<CourseRatingEnum>? Ratings { get; set; }
        public PriceEnum? Price { get; set; }
    }
}
