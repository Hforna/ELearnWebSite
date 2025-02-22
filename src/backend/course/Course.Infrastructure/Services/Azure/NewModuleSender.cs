using Azure.Messaging.ServiceBus;
using Course.Domain.Services.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Azure
{
    public class NewModuleSender : INewModuleSender
    {
        private readonly ServiceBusSender _serviceBus;

        public NewModuleSender(ServiceBusSender serviceBus) => _serviceBus = serviceBus;

        public async Task SendMessage(long moduleId)
        {
            await _serviceBus.SendMessageAsync(new ServiceBusMessage(moduleId.ToString()));
        }
    }
}
