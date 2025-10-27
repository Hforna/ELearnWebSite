using Course.Domain.DTOs;
using Course.Domain.Enums;
using Course.Domain.Services.Rest;
using Course.Exception;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Rest
{
    public class CurrencyFreaksService : ICurrencyExchangeService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string _apiKey;

        public CurrencyFreaksService(IHttpClientFactory httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<RateExchangeDto> GetCurrencyRates(CurrencyEnum currency)
        {
            var client = _httpClient.CreateClient();

            var currencyEnumList = new List<string>();
            foreach(var value in Enum.GetValues<CurrencyEnum>())
            {
                currencyEnumList.Add(value.ToString().ToLower());
            }
            var joinCurrencyList = string.Join(",", currencyEnumList);

            var request = await client.GetAsync($"https://api.currencyfreaks.com/v2.0/rates/latest?base={currency.ToString().ToLower()}&symbols={joinCurrencyList}&apikey={_apiKey}");

            var response = await request.Content.ReadAsStringAsync();
            if(request.IsSuccessStatusCode)
            {
                var toJson = JObject.Parse(response);
                var rates = toJson["rates"];

                return rates.ToObject<RateExchangeDto>();
            }
            throw new RestException(response);
        }
    }
}
