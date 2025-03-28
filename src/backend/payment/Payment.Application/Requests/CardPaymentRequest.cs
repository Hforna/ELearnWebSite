using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Requests
{
    public class CardPaymentRequest
    {
        public string CardToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Installments { get; set; }
        public bool OnCredit { get; set; } = false;
    }
}
