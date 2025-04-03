using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class Payout
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Active { get; set; } = true;
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public CurrencyEnum Currency { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; set; }
    }
}
