using Payment.Domain.DTOs;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.PaymentInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.PaymentAdapters
{
    public class TrioPixGatewayAdapter : IPixGatewayService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private const string BaseUrl = "https://api.trio.com.br";

        public TrioPixGatewayAdapter(IHttpClientFactory httpClient, string clientId, string clientSecret)
        {
            _httpClient = httpClient;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public async Task<PixPaymentResponseDto> ProcessPixTransaction(string cpf, string email, string firstName, string lastName, decimal amount)
        {
            var client = _httpClient.CreateClient(BaseUrl);
            var accessToken = await GetAccessToken();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var request = new
            {
                amount = amount * 100,
                payer = new
                {
                    name = $"{firstName} {lastName}",
                    document = cpf
                },
                expires_in = 1800
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/v1/pix/charges", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var id = doc.RootElement.GetProperty("id").GetString();
            var pixKey = doc.RootElement.GetProperty("pix").GetProperty("pix_key").ToString();
            var qrCode = doc.RootElement.GetProperty("pix").GetProperty("qr_code").ToString();
            var expiresOn = doc.RootElement.GetProperty("expires_at").GetDateTime();
            var status = doc.RootElement.GetProperty("status").ToString();


            return new PixPaymentResponseDto()
            {
                CodeToSend = pixKey,
                ExpiresOn = expiresOn,
                QrCodeBase64 = qrCode,
                Status = status,
                Id = id
            };
        }

        async Task<string> GetAccessToken()
        {
            var requestData = new FormUrlEncodedContent(new[]
            {
                 new KeyValuePair<string, string>("grant_type", "client_credentials"),
                 new KeyValuePair<string, string>("client_id", _clientId),
                 new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            var client = _httpClient.CreateClient();

            var request = await client.PostAsync($"{BaseUrl}/oauth2/token", requestData);

            var response = await request.Content.ReadAsStringAsync();

            if(request.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStreamAsync();
                using var doc = JsonDocument.Parse(content);

                return doc.RootElement.GetProperty("access_token").GetString()!;
            }
            throw new RestException(response, System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
