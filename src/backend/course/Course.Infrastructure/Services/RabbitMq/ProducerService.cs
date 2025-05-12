using Course.Domain.Services.RabbitMq;
using DnsClient.Internal;
using MassTransit;
using SharedMessages.CourseMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.RabbitMq
{
    public class ProducerService : IProducerService
    {
        private readonly IBus _bus;

        public ProducerService(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendCourseNote(CourseNoteMessage message)
        {
            await _bus.Publish(message, ctx =>
            {
                ctx.SetRoutingKey("course-note");
            });
        }

        public async Task SendCourseCreated(CourseCreatedMessage message)
        {
            await _bus.Publish(message, ctx =>
            {
                ctx.SetRoutingKey("course-created");
            });
        }
    }
}
