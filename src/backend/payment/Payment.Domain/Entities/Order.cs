using Payment.Domain.Cons;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("orders")]
    public class Order : BaseEntity
    {
        public long UserId { get; set; }
        public bool Active { get; set; } = true;
        public CurrencyEnum Currency { get; set; } = DefaultCurrency.Currency;
        public decimal TotalPrice { get; set; } = 0;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
