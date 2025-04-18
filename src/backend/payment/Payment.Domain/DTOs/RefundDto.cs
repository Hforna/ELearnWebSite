using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class RefundDto
    {
        public string GatewayId { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
    }
}
