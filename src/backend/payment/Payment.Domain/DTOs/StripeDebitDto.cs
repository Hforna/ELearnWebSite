using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public sealed record StripeDebitDto
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public bool RequiresAction { get; set; }
        public string ClientSecret { get; set; }
    }
}
