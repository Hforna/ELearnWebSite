using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Payment
{
    public abstract class PaymentBaseResponse
    {
        public Guid PaymentId { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public CurrencyEnum Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
