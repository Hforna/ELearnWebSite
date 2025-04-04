﻿using Payment.Domain.DTOs;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Services.Rest
{
    public interface ICurrencyExchangeService
    {
        public Task<RateExchangeDto> GetCurrencyRates(CurrencyEnum currency);
    }
}
