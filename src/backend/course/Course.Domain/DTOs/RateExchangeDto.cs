using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Course.Domain.DTOs
{
    public sealed record RateExchangeDto
    {
        [JsonPropertyName("brl")]
        public double BRL { get; set; }
        [JsonPropertyName("usd")]
        public double USD { get; set; }
        [JsonPropertyName("eur")]
        public double EUR { get; set; }
    }
}
