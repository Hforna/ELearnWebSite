using Azure.Messaging.ServiceBus;

namespace Course.Infrastructure.Services.Azure
{
    public class DeleteSender
    {
        private readonly ServiceBusSender _busSender;
         
        public DeleteSender(ServiceBusSender busSender) => _busSender = busSender;
    }
}