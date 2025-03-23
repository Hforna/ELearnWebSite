using Bogus;
using Course.Domain.DTOs;
using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.DTOs
{
    public static class CurrencyByLocationDtoTest
    {
        public static CurrencyByLocationDto Build()
        {
            return new Faker<CurrencyByLocationDto>()
                .RuleFor(d => d.Code, f => Enum.GetName(typeof(CurrencyEnum), f.PickRandom<CurrencyEnum>()))
                .RuleFor(d => d.Name, f => f.Commerce.ProductName())
                .RuleFor(d => d.Symbol, f => f.Lorem.Word());
        }
    }
}
