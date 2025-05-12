using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Dtos.RabbitMqMessages
{
    public class UserBoughtCourseMessage
    {
        public string courseId { get; set; }
        public string userId { get; set; }
    }
}
