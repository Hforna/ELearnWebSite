using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        [ForeignKey("PaymentId")]
        public PaymentEntity Payment { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TransactionStatusEnum TransactionStatus { get; set; }
    }
}
