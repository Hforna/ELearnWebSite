using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Requests
{
    public class PaymentBaseRequest
    {
        public Guid OrderId { get; set; }
    }
}
