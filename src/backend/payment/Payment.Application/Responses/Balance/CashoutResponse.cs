using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Balance
{
    public class CashoutResponse
    {
        public Guid Id { get; set; }
        public Guid BalanceId { get; set; }
        public Guid BankAccountId { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public decimal Amount { get; set; }
        public CurrencyEnum Currency { get; set; }

    }
}
