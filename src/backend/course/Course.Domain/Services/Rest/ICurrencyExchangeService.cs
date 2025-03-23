using Course.Domain.DTOs;
using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Rest
{
    public interface ICurrencyExchangeService
    {
        public Task<RateExchangeDto> GetCurrencyRates(CurrencyEnum currency);
    }
}
