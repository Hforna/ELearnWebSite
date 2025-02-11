using Azure.Messaging.ServiceBus;
using Course.Domain.Services.Azure;

namespace Course.Infrastructure.Services.Azure
{
    public class DeleteCourseSender : IDeleteCourseSender
    {
        private readonly ServiceBusSender _busSender;
         
        public DeleteCourseSender(ServiceBusSender busSender) => _busSender = busSender;

        public async Task SendMessage(long courseId)
        {
            await _busSender.SendMessageAsync(new ServiceBusMessage(courseId.ToString()));
        }
    }
}