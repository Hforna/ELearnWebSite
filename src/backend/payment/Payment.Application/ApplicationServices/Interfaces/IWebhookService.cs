using Payment.Application.Requests;
using Stripe;
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
        public Task CardStripeWebhook(Event payload);
        public Task BalanceTransferStripeWebhook(Event payload);
        public Task RefundOrderStripeWebhook(Event payload);
    }
}
