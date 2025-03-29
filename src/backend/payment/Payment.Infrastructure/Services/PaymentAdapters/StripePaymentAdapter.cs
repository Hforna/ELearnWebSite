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

        public async Task<StripeCreditDto> CreditCardPayment(string firstName, string lastName, string cardToken, decimal amount, CurrencyEnum currency, string userId, int installments = 1)
        {
            var currencyFormat = CurrencyFormat(currency);
            var options = new PaymentIntentCreateOptions()
            {
                Amount = (long)amount * 100,
                Currency = currencyFormat,
                PaymentMethod = cardToken,
                Confirm = true,
                Customer = userId,
                PaymentMethodOptions = new PaymentIntentPaymentMethodOptionsOptions()
                {
                    Card = new PaymentIntentPaymentMethodOptionsCardOptions()
                    {
                        Installments = new PaymentIntentPaymentMethodOptionsCardInstallmentsOptions()
                        {
                            Enabled = true,
                            Plan = new PaymentIntentPaymentMethodOptionsCardInstallmentsPlanOptions()
                            {
                                Count = installments,
                                Type = "fixed_count"
                            }
                        }
                    }
                },
                Metadata = new Dictionary<string, string>()
                {
                    { "installments", installments.ToString() }
                }
            };

            var service = new PaymentIntentService();

            var paymentIntent = await service.CreateAsync(options);

            return _mapper.Map<StripeCreditDto>(paymentIntent);
        }

        public async Task<StripeDebitDto> DebitCardPayment(string firstName, string lastName, string cardToken, decimal amount, CurrencyEnum currency, string userId)
        {
            var currencyFormat = CurrencyFormat(currency);
            var options = new PaymentIntentCreateOptions()
            {
                Amount = (long?)amount * 100,
                Currency = currencyFormat,
                Customer = userId,
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

        string CurrencyFormat(CurrencyEnum currency) => currency.ToString().ToLower();
    }
}
