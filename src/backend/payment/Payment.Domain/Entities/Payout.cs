using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("payouts")]
    public class Payout
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Active { get; set; } = true;
        public long UserId { get; set; }
        public decimal Amount { get; set; } = 0;
        public TransactionStatusEnum TransactionStatus { get; set; }
        public CurrencyEnum Currency { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; set; }
    }
}
