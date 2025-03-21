using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Payment.Domain.DTOs;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.Rest
{
    public class GeoLocationRestService : ILocationRestService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;

        public GeoLocationRestService(IHttpClientFactory httpClient, IConfiguration configuration)
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

            if(request.IsSuccessStatusCode)
            {
                var toJson = JObject.Parse(response);
                var currencyToken = toJson["currency"];

                var infos = currencyToken.ToObject<CurrencyByLocationDto>();
                return infos;
            }
            throw new RestException(response, System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
