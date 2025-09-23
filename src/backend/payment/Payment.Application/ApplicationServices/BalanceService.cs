using AutoMapper;
using FluentValidation.Internal;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Extensions;
using Payment.Application.Requests;
using Payment.Application.Responses.Balance;
using Payment.Application.Services;
using Payment.Domain.Cons;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Payment.PaymentInterfaces;
using Payment.Domain.Services.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices
{
    public class BalanceService : IBalanceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IUserRestService _userRest;
        private readonly ILocationRestService _locationRest;
        private readonly ICurrencyExchangeService _exchangeService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IPaymentGatewayService _paymentService;
        private readonly ILogger<BalanceService> _logger;


        public BalanceService(IMapper mapper, IUnitOfWork uof, IUserRestService userRest, 
            ILocationRestService locationRest, ICurrencyExchangeService exchangeService, 
            SqidsEncoder<long> sqids, IPaymentGatewayService paymentGateway, ILogger<BalanceService> logger)
        {
            _mapper = mapper;
            _uof = uof;
            _logger = logger;
            _paymentService = paymentGateway;
            _userRest = userRest;
            _locationRest = locationRest;
            _exchangeService = exchangeService;
            _sqids = sqids;
        }

        public async Task<BankAccountResponse> CreateBankAccount(CreateBankAccountRequest request)
        {
            var validator = new CreateBankAccountRequestValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new BalanceException(errorMessages, System.Net.HttpStatusCode.BadRequest);
            }

            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var bankAccount = _mapper.Map<UserBankAccount>(request);
            bankAccount.TeacherId = userId;

            await _uof.bankAccountWrite.Add(bankAccount);
            await _uof.Commit();

            return _mapper.Map<BankAccountResponse>(bankAccount);
        }

        public async Task<BalanceResponse> GetBalance()
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var balance = await _uof.balanceRead.BalanceByTeacherId(userId);

            if (balance is null)
                throw new PaymentException(ResourceExceptMessages.BALANCE_DOESNT_EXISTS, System.Net.HttpStatusCode.InternalServerError);

            var response = _mapper.Map<BalanceResponse>(balance);
            var blockedBalanceAmount = await _uof.balanceRead.GetBlockedBalanceAmount(balance.Id);

            var userCurrency = await CurrencyExtensions.GetCurrency(_locationRest);
            decimal rate = await CalcCurrencyRates(balance.Currency, userCurrency);

            response.Currency = userCurrency;
            response.BlockedBalance = blockedBalanceAmount is not null ? blockedBalanceAmount * rate : null;
            response.AvaliableBalance *= rate;

            return response;
        }

        public async Task<CashoutResponse> UserCashOut(CashoutRequest request)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userBalance = await _uof.balanceRead.BalanceByTeacherId(userId);

            if (userBalance is null)
                throw new BalanceException(ResourceExceptMessages.BALANCE_NOT_EXISTS_OR_TRANSACTION_PENDING, System.Net.HttpStatusCode.NotFound);

            var bankAccount = await _uof.bankAccountRead.UserBankAccountByIdAndUserId(request.BankAccountId, userId);

            if (bankAccount is null)
                throw new BalanceException(ResourceExceptMessages.USER_BANK_NOT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var recentPayouts = await _uof.payoutRead.PayoutRecentsByUserAndTime(userId, DateTime.UtcNow);

            if (recentPayouts is not null)
                throw new BalanceException(ResourceExceptMessages.PAYOUT_MADE_FEW_TIMES_AGO, System.Net.HttpStatusCode.Unauthorized);

            var userCurrency = await _locationRest.GetCurrencyByUserLocation();
            var currencyAsEnum = Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, out var result) ? (CurrencyEnum)result : DefaultCurrency.Currency;
            decimal rate = await CalcCurrencyRates(userBalance.Currency, currencyAsEnum);

            decimal amount = Math.Round(request.Amount * rate, 2);

            if (amount > userBalance.AvaliableBalance)
                throw new BalanceException(ResourceExceptMessages.INVALID_AMOUNT_IN_BALANCE, System.Net.HttpStatusCode.Unauthorized);

            var transfer = await _paymentService.CashoutAsTedMethod(request.Amount, userId, userCurrency.Name, currencyAsEnum,
                bankAccount.AccountNumber, bankAccount.AgencyNumber, bankAccount.TypeAccount,
                bankAccount.TaxId, bankAccount.FirstName, bankAccount.LastName, bankAccount.Email);

            var payout = new Payout()
            {
                Currency = currencyAsEnum,
                UserId = userId,
                Amount = amount
            };

            switch (transfer.Status)
            {
                case StripeCashOutDto.Failed:
                    payout.Active = false;
                    payout.TransactionStatus = TransactionStatusEnum.Canceled;
                    await _uof.payoutWrite.Add(payout);
                    await _uof.Commit();

                    _logger.LogError($"It was not possible to process user cashout from balance: {userBalance.Id}");

                    throw new BalanceException(ResourceExceptMessages.PAYOUT_FAILED, System.Net.HttpStatusCode.InternalServerError);
                case StripeCashOutDto.Pending:
                    payout.TransactionStatus = TransactionStatusEnum.Pending;
                    break;
                case StripeCashOutDto.Accepted:
                    payout.TransactionStatus = TransactionStatusEnum.Approved;
                    break;
            }

            userBalance.AvaliableBalance -= amount;
            _uof.balanceWrite.Update(userBalance);
            await _uof.payoutWrite.Add(payout);

            await _uof.Commit();

            return new CashoutResponse()
            {
                Amount = request.Amount,
                Currency = currencyAsEnum,
                BalanceId = userBalance.Id,
                Id = payout.Id,
                BankAccountId = bankAccount.Id,
                Status = payout.TransactionStatus
            };
        }
        private async Task<decimal> CalcCurrencyRates(CurrencyEnum currentSource, CurrencyEnum currencyTarget)
        {
            decimal rates = 0;

            var currencyRates = await _exchangeService.GetCurrencyRates(currentSource);

            switch (currencyTarget)
            {
                case CurrencyEnum.USD:
                    rates = currencyRates.USD;
                    break;
                case CurrencyEnum.EUR:
                    rates = currencyRates.EUR;
                    break;
                case CurrencyEnum.BRL:
                    rates = currencyRates.BRL;
                    break;
            }

            return rates;
        }
    }
}
