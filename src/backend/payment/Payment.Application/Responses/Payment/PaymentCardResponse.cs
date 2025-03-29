using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Payment
{
    public class PaymentCardResponse : PaymentBaseResponse
    {
        public decimal Amount { get; set; }
        public int? Installments { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public bool RequiresAction { get; set; }
        public string ClientSecret { get; set; }
    }
}
