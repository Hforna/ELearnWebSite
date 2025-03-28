using AutoMapper;
using Payment.Domain.DTOs;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Payment;
using Payment.Domain.Services.Payment.PaymentInterfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Services.PaymentAdapters
{
    public class StripePaymentAdapter : IPaymentGatewayService
    {
        private readonly IMapper _mapper;

        public StripePaymentAdapter(IMapper mapper) => _mapper = mapper;

        public async Task<StripeDebitDto> DebitCardPayment(string firstName, string lastName, string cardToken, decimal amount, CurrencyEnum currency)
        {
            var currencyFormat = currency.ToString().ToLower();
            var options = new PaymentIntentCreateOptions()
            {
                Amount = (long?)amount,
                Currency = currencyFormat,
                PaymentMethod = cardToken,
                Confirm = true,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions()
                {
                    AllowRedirects = "never",
                    Enabled = true
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var response = _mapper.Map<StripeDebitDto>(paymentIntent);

            return response;
        }
    }
}
