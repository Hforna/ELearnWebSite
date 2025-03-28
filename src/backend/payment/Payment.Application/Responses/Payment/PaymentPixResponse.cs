using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Payment
{
    public class PaymentPixResponse
    {
        public Guid PaymentId { get; set; }
        public Guid TransactionId { get; set; }
        public string? GatewayId { get; set; }
        public string QrCode { get; set; }
        public string Hash { get; set; }
        public string Status { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
