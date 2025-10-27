using Course.Domain.DTOs;
using Course.Domain.Services.Rest;
using Course.Exception;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Rest
{
    public class GeoLocationService : ILocationService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;

        public GeoLocationService(IHttpClientFactory httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<CurrencyByLocationDto> GetCurrencyByUserLocation()
        {
            var apiKey = _configuration.GetValue<string>("apis:geoLocation:apiKey");

            var client = _httpClient.CreateClient();
            var request = await client.GetAsync($"https://api.ipgeolocation.io/ipgeo?apiKey={apiKey}&fields=currency");

            var response = await request.Content.ReadAsStringAsync();

            if (request.IsSuccessStatusCode)
            {
                var toJson = JObject.Parse(response);
                var currencyToken = toJson["currency"];

                var infos = currencyToken.ToObject<CurrencyByLocationDto>();
                return infos;
            }
            throw new RestException(response);
        }
    }
}
