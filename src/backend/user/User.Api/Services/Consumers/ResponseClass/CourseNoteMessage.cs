using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace User.Api.Services.Consumers.ResponseClass
{
    public class CourseNoteMessage
    {
        public int CourseNumber { get; set; }
        public string UserId { get; set; }
        public decimal Note { get; set; }
    }
}
