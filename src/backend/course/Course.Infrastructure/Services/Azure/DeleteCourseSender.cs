using Azure.Messaging.ServiceBus;
using Course.Domain.Services.Azure;

namespace Course.Infrastructure.Services.Azure
{
    public class DeleteSender : IDeleteSender
    {
        private readonly ServiceBusSender _busSender;
         
        public DeleteSender(ServiceBusSender busSender) => _busSender = busSender;

        public async Task SendMessage(long courseId)
        {
            await _busSender.SendMessageAsync(new ServiceBusMessage(courseId.ToString()));
        }
    }
}