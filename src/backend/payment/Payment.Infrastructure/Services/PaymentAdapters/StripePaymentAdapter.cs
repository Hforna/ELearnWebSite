using AutoMapper;
using MassTransit.Saga;
using Payment.Domain.DTOs;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Payment;
using Payment.Domain.Services.Payment.PaymentInterfaces;
using Stripe;
using Stripe.FinancialConnections;
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

        public async Task<StripeCashOutDto> CashoutAsTedMethod(decimal amount, long userId, string countryCode, CurrencyEnum currency,
        string accountName, string agencyNumber, string accountType, string taxId,
        string firstName, string lastName, string email)
        {
            try
            {
                var account = CreateUserAccount(countryCode.ToUpper(), email, firstName, lastName, taxId);

                var bankAccountOptions = new AccountExternalAccountBankAccountOptions()
                {
                    AccountHolderName = $"{firstName} {lastName}".Trim(),
                    AccountHolderType = "individual",
                    Country = countryCode,
                    Currency = currency.ToString().ToLower(),
                    RoutingNumber = agencyNumber,
                    AccountNumber = accountName,
                };

                var externalAccountService = new AccountExternalAccountService();
                var bankAccount = await externalAccountService.CreateAsync(
                    account.Id,
                    new AccountExternalAccountCreateOptions
                    {
                        ExternalAccount = bankAccountOptions
                    }
                );

                var transferService = new TransferService();
                var transfer = await transferService.CreateAsync(new TransferCreateOptions
                {
                    Amount = Convert.ToInt64(amount * 100),
                    Currency = currency.ToString().ToLower(),
                    Destination = account.Id,
                    Description = $"TED transfer for user {userId}",
                    Metadata = new Dictionary<string, string>()
                    {
                        {"user_id", userId.ToString() }
                    }
                });

                return new StripeCashOutDto()
                {
                    GatewayId = transfer.Id,
                    Status = transfer.SourceTransaction.Status
                };
            }
            catch (StripeException e)
            {
                throw new Exception($"Stripe error: {e.StripeError.Message}");
            }
        }

        public async Task<StripeCreditDto> CreditCardPayment(string firstName, string lastName, string cardToken, 
            decimal amount, CurrencyEnum currency, string userId, int installments = 1)
        {
            var currencyFormat = CurrencyFormat(currency);
            var options = new PaymentIntentCreateOptions()
            {
                Amount = (long)amount * 10,
                Currency = currencyFormat,
                PaymentMethod = cardToken,
                Confirm = true,
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
                    { "installments", installments.ToString() },
                    { "user_id", userId },
                    { "first_name", firstName },
                    { "last_name", lastName }
                }
            };

            var service = new PaymentIntentService();

            var paymentIntent = await service.CreateAsync(options);

            return _mapper.Map<StripeCreditDto>(paymentIntent);
        }

        public async Task<StripeDebitDto> DebitCardPayment(string firstName, string lastName, string cardToken, 
            decimal amount, CurrencyEnum currency, string userId)
        {
            var currencyFormat = CurrencyFormat(currency);
            var options = new PaymentIntentCreateOptions()
            {
                Amount = (long?)amount * 10,
                Currency = currencyFormat,
                PaymentMethod = "pm_card_visa",
                Confirm = true,
                Metadata = new Dictionary<string, string>()
                {
                    { "user_id", userId },
                    { "first_name", firstName },
                    { "last_name", lastName }
                },
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions()
                {
                    AllowRedirects = "never",
                    Enabled = true
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var response = _mapper.Map<StripeDebitDto>(paymentIntent);
            response.Success = paymentIntent.Status == "succeeded";

            return response;
        }

        public async Task<RefundDto> RefundUserCourse(long courseId, long userId, decimal amount, CurrencyEnum currency, string paymentIntentId)
        {
            var options = new RefundCreateOptions()
            {
                PaymentIntent = paymentIntentId,
                Amount = (long)amount * 10,
                Currency = currency.ToString().ToLower(),
                Metadata = new Dictionary<string, string>()
                {
                    { "course_id", courseId.ToString() },
                    { "user_id", userId.ToString() }
                },
                Reason = RefundReasons.RequestedByCustomer
            };

            var refundService = await new RefundService().CreateAsync(options);

            return _mapper.Map<RefundDto>(refundService);
        }

        Stripe.Account? CreateUserAccount(string country, string email, string firstName, string lastName, string taxId)
        {
            var accountOptions = new AccountCreateOptions
            {
                Type = "custom",
                Country = country,
                Email = email,
                BusinessType = "individual",
                Individual = new AccountIndividualOptions
                {
                    FirstName = firstName,
                    LastName = lastName,
                    IdNumber = taxId,
                },
                Capabilities = new AccountCapabilitiesOptions
                {
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                },
            };

            var accountService = new Stripe.AccountService();
            var account = accountService.Create(accountOptions);

            return account;
        }

        string CurrencyFormat(CurrencyEnum currency) => currency.ToString().ToLower();
    }
}
