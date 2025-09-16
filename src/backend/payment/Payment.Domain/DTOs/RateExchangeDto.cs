using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public sealed record RateExchangeDto
    {
        [JsonPropertyName("brl")]
        public decimal BRL { get; set; }
        [JsonPropertyName("usd")]
        public decimal USD { get; set; }
        [JsonPropertyName("eur")]
        public decimal EUR { get; set; }
    }
}
