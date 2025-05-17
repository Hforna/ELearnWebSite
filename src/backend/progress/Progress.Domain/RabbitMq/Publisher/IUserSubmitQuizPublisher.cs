using Progress.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.RabbitMq.Publisher
{
    public interface IUserSubmitQuizPublisher
    {
        public Task SendMessage(UserSubmitDto message);
    }
}
