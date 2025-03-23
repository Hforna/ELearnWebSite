using Bogus;
using Course.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.DTOs
{
    public static class RateExchangeDtoTest
    {
        public static RateExchangeDto Build()
        {
            return new Faker<RateExchangeDto>()
                .RuleFor(d => d.USD, f => (double)f.Finance.Amount(1, 20, 5))
                .RuleFor(d => d.BRL, f => (double)f.Finance.Amount(1, 20, 5))
                .RuleFor(d => d.EUR, f => (double)f.Finance.Amount(1, 20, 5));
        }
    }
}
