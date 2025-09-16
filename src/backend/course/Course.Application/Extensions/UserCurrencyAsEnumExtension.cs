using Course.Domain.Enums;
using Course.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Extensions
{
    public static class UserCurrencyAsEnumExtension
    {
        public static async Task<CurrencyEnum> GetCurrency(ILocationService locationRest)
        {
            var userCurrency = await locationRest.GetCurrencyByUserLocation();
            return Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, true, out var result) ? (CurrencyEnum)result : CurrencyEnum.BRL;
        }
    }
}
