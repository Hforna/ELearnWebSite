using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public sealed record PixPaymentResponseDto
    {
        public string? Id { get; set; }
        public string Status { get; set; }
        public string CodeToSend { get; set; }
        public string QrCodeBase64 { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
