using Payment.Domain.DTOs;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.PaymentInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public async Task<PixPaymentResponseDto> ProcessPixTransaction(string cpf, string email, string firstName, string lastName, decimal amountOrder)
        {
            var client = _httpClient.CreateClient();

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var request = new
            {
                virtual_account_id = "0195cdf4-fa45-9e9e-8da2-f8db7a28c45b",
                counterparty = new
                {
                    tax_number = cpf,
                    name = $"{firstName} {lastName}"
                },
                amount = Math.Round(amountOrder) * 100,
                external_id = Guid.NewGuid().ToString().ToUpper(),
                description = "pay course",
                expiration_datetime = DateTime.UtcNow.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                options = new
                {
                    allow_change_the_amount_on_payment = false,
                    show_qrcode_image = false
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"https://api.sandbox.trio.com.br/banking/cashin/pix/qrcodes", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var data = doc.RootElement.GetProperty("data");
            var id = data.GetProperty("id").GetString();
            var qrCode = data.GetProperty("hash").GetString();
            var expiresOn = data.GetProperty("expiration_datetime").GetDateTime();
            var status = data.GetProperty("status").GetString();


            return new PixPaymentResponseDto()
            {
                Hash = qrCode,
                ExpiresOn = expiresOn,
                QrCodeBase64 = qrCode,
                Status = status,
                Id = id
            };
        }
    }
}
