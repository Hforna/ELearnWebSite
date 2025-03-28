﻿using Payment.Domain.DTOs;
using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Services.Payment.PaymentInterfaces
{
    public interface IPaymentGatewayService
    {
        public Task<StripeDebitDto> DebitCardPayment(string firstName, string lastName, string cardToken, decimal amount, CurrencyEnum currency, string userId);
        public Task<StripeCreditDto> CreditCardPayment(string firstName, string lastName, string cardToken, decimal amount, CurrencyEnum currency, string userId, int installments = 1);
    }
}
