using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public string GatewayTransactionId { get; set; }
        public CurrencyEnum Currency { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
    }
}
