using Payment.Domain.Cons;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("balances")]
    public class Balance
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public long TeacherId { get; set; }
        public decimal AvaliableBalance { get; set; } = 0;
        public CurrencyEnum Currency { get; set; } = DefaultCurrency.Currency;
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
