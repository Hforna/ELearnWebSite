using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Requests
{
    public class CashoutRequest
    {
        public decimal Amount { get; set; }
        public Guid BankAccountId { get; set; }
    }
}
