using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class StripeCashOutDto
    {
        public string Status { get; set; }
        public string GatewayId { get; set; }
    }
}
