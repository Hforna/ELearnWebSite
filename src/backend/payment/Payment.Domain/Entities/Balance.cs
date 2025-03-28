using Payment.Domain.Cons;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    public class Balance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public long TeacherId { get; set; }
        public decimal AvaliableBalance { get; set; }
        public decimal BlockedBalance { get; set; }
        public CurrencyEnum Currency { get; set; } = DefaultCurrency.Currency;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; }
    }
}
