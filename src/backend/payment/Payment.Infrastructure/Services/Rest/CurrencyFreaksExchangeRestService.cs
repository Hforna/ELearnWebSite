using Newtonsoft.Json.Linq;
using Payment.Domain.DTOs;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.Rest
{
    public class CurrencyFreaksExchangeRestService : ICurrencyExchangeService
    {
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClient;

        public CurrencyFreaksExchangeRestService(string apiKey, IHttpClientFactory httpClient)
        {
            _apiKey = apiKey;
            _httpClient = httpClient;
        }

        public async Task<RateExchangeDto> GetCurrencyRates(CurrencyEnum currency)
        {
            var client = _httpClient.CreateClient();

            List<string> currencysAccepted = new List<string>();

            foreach(var value in Enum.GetValues<CurrencyEnum>())
            {
                currencysAccepted.Add(value.ToString());
            }
            var currencysOnString = string.Join(",", currencysAccepted).Trim();

            var request = await client
                .GetAsync($"https://api.currencyfreaks.com/v2.0/rates/latest?base={currency.ToString().ToLower()}&symbols={currencysOnString}&apikey={_apiKey}");

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var toJson = JObject.Parse(response);
                var ratesToken = toJson["rates"];

                return ratesToken.ToObject<RateExchangeDto>();
            }
            throw new RestException(response, System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
