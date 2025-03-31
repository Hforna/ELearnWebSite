using Bogus;
using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.DTOs
{
    public static class ExchangeRatesDtoFaker
    {
        public static RateExchangeDto Build()
        {
            return new Faker<RateExchangeDto>()
                .RuleFor(d => d.BRL, f => (double)f.Finance.Amount(1, 5, 2))
                .RuleFor(d => d.EUR, f => (double)f.Finance.Amount(1, 5, 2))
                .RuleFor(d => d.USD, f => (double)f.Finance.Amount(1, 5, 2));
        }
    }
}
