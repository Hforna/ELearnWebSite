using CommonUtilities.Fakers.DTOs;
using Moq;
using Payment.Domain.Enums;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Services
{
    public class CurrencyExchangeMock
    {
        private readonly Mock<ICurrencyExchangeService> _mock = new Mock<ICurrencyExchangeService>();

        public ICurrencyExchangeService Build() => _mock.Object;
        public void GetCurrencyRates(CurrencyEnum currency) => _mock.Setup(d => d.GetCurrencyRates(currency)).ReturnsAsync(ExchangeRatesDtoFaker.Build());

    }
}
