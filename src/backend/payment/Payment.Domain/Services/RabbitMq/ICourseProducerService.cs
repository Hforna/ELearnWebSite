using SharedMessages.PaymentMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Services.RabbitMq
{
    public interface ICourseProducerService
    {
        public Task SendAllowCourseToUser(AllowCourseToUserMessage message);
    }
}
