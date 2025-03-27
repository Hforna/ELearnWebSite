using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Requests
{
    public class CardPaymentRequest : PaymentBaseRequest
    {
        public string CardToken { get; set; }
        public int Installments { get; set; }
        public bool Credit { get; set; } = false;
    }
}
