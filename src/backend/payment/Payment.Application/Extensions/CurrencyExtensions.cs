using Payment.Domain.Cons;
using Payment.Domain.Enums;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Extensions
{
    public static class CurrencyExtensions
    {
        public static async Task<CurrencyEnum> GetCurrency(ILocationRestService locationRest)
        {
            var userCurrency = await locationRest.GetCurrencyByUserLocation();
            return Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, true, out var result) ? (CurrencyEnum)result : DefaultCurrency.Currency;
        }
    }
}
