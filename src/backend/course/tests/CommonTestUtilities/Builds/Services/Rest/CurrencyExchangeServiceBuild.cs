using Course.Communication.Enums;
using Course.Domain.DTOs;
using Course.Domain.Services.Rest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Rest
{
    public class CurrencyExchangeServiceBuild
    {
        private readonly Mock<ICurrencyExchangeService> _mock = new Mock<ICurrencyExchangeService>();

        public ICurrencyExchangeService Build() => _mock.Object;

        public void CurrencyRates(RateExchangeDto rateExchange)
        {
            _mock.Setup(d => d.GetCurrencyRates(It.IsAny<Course.Domain.Enums.CurrencyEnum>())).ReturnsAsync(rateExchange);
        }
    }
}
