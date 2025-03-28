using MassTransit;
using Payment.Domain.Services.RabbitMq;
using SharedMessages.PaymentMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.RabbitMq
{
    public class CourseProducerService : ICourseProducerService
    {
        private readonly IBus _bus;

        public CourseProducerService(IBus bus)
        {
            _bus = bus;
        }

        public async Task SendAllowCourseToUser(AllowCourseToUserMessage message)
        {
            await _bus.Publish(message, cfg =>
            {
                cfg.SetRoutingKey("allow.course");
            });
        }
    }
}
