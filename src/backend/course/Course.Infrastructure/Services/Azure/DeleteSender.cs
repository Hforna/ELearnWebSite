namespace Course.Infrastructure.Services.Azure
{
    public class DeleteSender
    {
        private readonly ServiceBusClient _busClient;
         
        public DeleteSender(ServiceBusClient busClient) => _busClient = busClient;
    }
}