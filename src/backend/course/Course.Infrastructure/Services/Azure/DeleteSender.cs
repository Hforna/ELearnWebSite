using Azure.Messaging.ServiceBus;
using Course.Domain.Services.Azure;

namespace Course.Infrastructure.Services.Azure
{
    public class DeleteSender : IDeleteSender
    {
        private readonly ServiceBusSender _busSender;
         
        public DeleteSender(ServiceBusSender busSender) => _busSender = busSender;

        public Task SendMessage(long courseId)
        {
            _busSender.SendMessagesAsync("deleteCourse", courseId.ToString());
        }
    }
}