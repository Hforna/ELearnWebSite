using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Azure
{
    public class NewModuleProcessor
    {
        private readonly ServiceBusProcessor _processor;

        public NewModuleProcessor(ServiceBusProcessor processor) => _processor = processor;

        public ServiceBusProcessor GetProcessor() => _processor;
    }
}
