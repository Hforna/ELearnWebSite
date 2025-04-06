using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("blocked-balances")]
    public class BlockedBalance : BaseEntity
    {
        public decimal Amount { get; set; } = 0;
        [ForeignKey("BalanceId")]
        public Balance Balance { get; set; }
        public Guid BalanceId { get; set; }
    }
}
