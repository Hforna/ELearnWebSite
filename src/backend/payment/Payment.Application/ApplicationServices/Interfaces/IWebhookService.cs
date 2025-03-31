using Payment.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices.Interfaces
{
    public interface IWebhookService
    {
        public Task PixTrioWebhook(TrioWebhookPayload payload);
    }
}
