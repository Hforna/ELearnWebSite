using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.Rest
{
    public class RateCurrencyExchangeRestService
    {
        private readonly string _apiKey;
        private readonly IHttpClientFactory _httpClient;

        public RateCurrencyExchangeRestService(string apiKey, IHttpClientFactory httpClient)
        {
            _apiKey = apiKey;
            _httpClient = httpClient;
        }
    }
}
