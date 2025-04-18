using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Payment
{
    public class RefundResponse
    {
        public string CourseId { get; set; }
        public string GatewayId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
