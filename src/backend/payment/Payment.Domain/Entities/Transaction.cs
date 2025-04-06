using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("transactions")]
    public class Transaction : BaseEntity
    {
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
        public decimal Amount { get; set; } = 0;
        public DateTime? UpdatedOn { get; set; }
        public string GatewayTransactionId { get; set; }
        public CurrencyEnum Currency { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }
    }
}
