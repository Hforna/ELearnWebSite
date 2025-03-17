using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class PaymentEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public CurrencyEnum Currency { get; set; }
        public long CustomerId { get; set; }
        public PaymentMethodEnum PaymentMethodType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? TokenizedData { get; set; }
    }
}
