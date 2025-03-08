using Course.Communication.Requests.MessageSenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.RabbitMq
{
    public interface IUserSenderService
    {
        public Task SendCourseNote(CourseNoteMessage message);
    }
}
