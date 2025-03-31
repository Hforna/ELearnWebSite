using AutoMapper;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Http;
using Payment.Domain.DTOs;
using Payment.Domain.Services.PaymentInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.PaymentAdapters
{
    public class MercadoPagoGatewayAdapter : IPixGatewayService
    {
        public async Task<PixPaymentResponseDto> ProcessPixTransaction(string cpf, string email, string firstName, string lastName, decimal amount, string? userId = null)
        {
            var requestOptions = new RequestOptions();

            var request = new PaymentCreateRequest()
            {
                TransactionAmount = amount,
                DateOfExpiration = DateTime.UtcNow.AddMinutes(30),
                Description = "Elearn courses",
                PaymentMethodId = "pix",
                Payer = new PaymentPayerRequest()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Identification = new MercadoPago.Client.Common.IdentificationRequest()
                    {
                        Type = "CPF",
                        Number = cpf
                    }
                }
            };

            var client = new PaymentClient();
            var payment = await client.CreateAsync(request, requestOptions);
            return new PixPaymentResponseDto()
            {
                Hash = payment.PointOfInteraction.TransactionData.QrCode,
                Id = payment.Id.ToString(),
                QrCodeBase64 = payment.PointOfInteraction.TransactionData.QrCodeBase64,
                Status = payment.Status,
                ExpiresOn = (DateTime)payment.DateOfExpiration!
            };
        }
    }
}
