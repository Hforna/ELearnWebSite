using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.DTOs
{
    public class CurrencyByLocationDto
    {
        public string name { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
    }
}
