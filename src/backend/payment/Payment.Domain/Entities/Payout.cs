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
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
