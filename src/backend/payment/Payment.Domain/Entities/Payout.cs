using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Domain.Exceptions;

namespace Payment.Domain.Entities
{
    [Table("payouts")]
    public class Payout
    {
        public Payout(bool active, long userId, 
            decimal amount, TransactionStatusEnum transactionStatus, CurrencyEnum currency, DateTime processedAt)
        {
            Active = active;
            UserId = userId;
            Amount = amount;
            TransactionStatus = transactionStatus;
            Currency = currency;
            ProcessedAt = processedAt;
        }

        public void PayoutIsProcessed(TransactionStatusEnum  transactionStatus)
        {
            if (transactionStatus == TransactionStatusEnum.Processing ||
                transactionStatus == TransactionStatusEnum.Pending)
                throw new DomainException();
            
            ProcessedAt = DateTime.UtcNow;
            
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public bool Active { get; set; } = true;
        public long UserId { get; set; }
        public decimal Amount { get; set; } = 0;
        public TransactionStatusEnum TransactionStatus { get; set; }
        public CurrencyEnum Currency { get; set; }
        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
        public DateTime ProcessedAt { get; set; }
    }

    public class PayoutPolicy
    {
        public bool CanRequestPayout(DateTime lastPayout)
        {
            return lastPayout.AddDays(1) > DateTime.UtcNow;
        }
    }
}
