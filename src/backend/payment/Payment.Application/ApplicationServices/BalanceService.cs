using AutoMapper;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Extensions;
using Payment.Application.Requests;
using Payment.Application.Responses.Balance;
using Payment.Domain.Cons;
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


        public BalanceService(IMapper mapper, IUnitOfWork uof, IUserRestService userRest, 
            ILocationRestService locationRest, ICurrencyExchangeService exchangeService, 
            SqidsEncoder<long> sqids, IPaymentGatewayService paymentGateway)
        {
            _mapper = mapper;
            _uof = uof;
            _paymentService = paymentGateway;
            _userRest = userRest;
            _locationRest = locationRest;
            _exchangeService = exchangeService;
            _sqids = sqids;
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

            var userCurrency = await UserCurrencyAsEnumExtension.GetCurrency(_locationRest);
            var currencyRates = await _exchangeService.GetCurrencyRates(balance.Currency);

            decimal rate = 0;

            switch(userCurrency)
            {
                case Domain.Enums.CurrencyEnum.BRL:
                    rate = (decimal)currencyRates.BRL;
                    break;
                case Domain.Enums.CurrencyEnum.EUR:
                    rate = (decimal)currencyRates.EUR;
                    break;
                case Domain.Enums.CurrencyEnum.USD:
                    rate = (decimal)currencyRates.USD;
                    break;
            }

            response.Currency = userCurrency;
            response.BlockedBalance = blockedBalanceAmount is not null ? (decimal)blockedBalanceAmount * rate : null;
            response.AvaliableBalance *= rate;

            return response;
        }

        public async Task UserCashOut(CashoutRequest request)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userBalance = await _uof.balanceRead.BalanceByTeacherId(userId);

            if (userBalance is null)
                throw new BalanceException(ResourceExceptMessages.BALANCE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            if (request.Amount > userBalance.AvaliableBalance)
                throw new BalanceException(ResourceExceptMessages.INVALID_AMOUNT_IN_BALANCE, System.Net.HttpStatusCode.Unauthorized);

            var bankAccount = await _uof.bankAccountRead.UserBankAccountByIdAndUserId(request.BankAccountId, userId);

            if (bankAccount is null)
                throw new BalanceException();

            var userCurrency = await _locationRest.GetCurrencyByUserLocation();
            var currencyAsEnum = Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, out var result) ? (CurrencyEnum)result : DefaultCurrency.Currency;

            var rates = 0.0;
            var currencyRates = await _exchangeService.GetCurrencyRates(userBalance.Currency);

            switch(currencyAsEnum)
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

            decimal amount = Math.Round((decimal)request.Amount * (decimal)rates, 2);

            var transfer = await _paymentService.CashoutAsTedMethod(amount, userId, userCurrency.Name, currencyAsEnum, 
                bankAccount.AccountNumber, bankAccount.AgencyNumber, bankAccount.TypeAccount, 
                bankAccount.TaxId, bankAccount.FirstName, bankAccount.LastName, bankAccount.Email);

            var payout = new Payout()
            {
                Currency = currencyAsEnum,
                UserId = userId,
                
            }

            if (transfer.Status == "failed")
                throw new BalanceException(System.Net.HttpStatusCode.InternalServerError);

            if(transfer.Status == "succeeded")
            {

            }
        }
    }
}
