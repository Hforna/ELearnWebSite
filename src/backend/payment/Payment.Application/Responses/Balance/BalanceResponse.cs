using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Balance
{
    public class BalanceResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public decimal AvaliableBalance { get; set; }
        public decimal? BlockedBalance { get; set; }
        public CurrencyEnum Currency { get; set; }
    }
}
