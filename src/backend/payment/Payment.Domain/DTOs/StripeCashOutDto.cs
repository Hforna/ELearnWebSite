using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class StripeCashOutDto
    {
        public const string Pending = "pending";
        public const string Failed = "failed";
        public const string Accepted = "accepted";
        public const string Processing = "processing";

        public string Status { get; set; }
        public string GatewayId { get; set; }
    }
}
