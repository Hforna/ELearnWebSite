using Bogus;
using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.DTOs
{
    public class CurrencyByLocationDtoFaker
    {
        public CurrencyByLocationDto Build()
        {
            var currencys = new List<string>() { "BRL", "USD", "EUR" };
            var symbols = new List<string>() { "$", "€", "R$" };

            return new Faker<CurrencyByLocationDto>()
                .RuleFor(d => d.Code, f => f.PickRandom(currencys))
                .RuleFor(d => d.Symbol, f => f.PickRandom(symbols))
                .RuleFor(d => d.Name, f => f.PickRandom(currencys));
        }
    }
}
