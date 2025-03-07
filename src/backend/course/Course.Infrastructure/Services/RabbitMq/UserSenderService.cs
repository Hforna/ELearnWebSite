using Course.Communication.Requests.MessageSenders;
using Course.Domain.Services.RabbitMq;
using DnsClient.Internal;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.RabbitMq
{
    public class UserSenderService : IUserSenderService
    {
        private readonly IBus _bus;

        public UserSenderService(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendCourseNote(SendCourseNoteMessage message)
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri("queue:course-note-queue"));
            await sendEndpoint.Send<SendCourseNoteMessage>(message);
        }
    }
}
