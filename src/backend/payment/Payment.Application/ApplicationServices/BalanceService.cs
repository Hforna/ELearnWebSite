using AutoMapper;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Extensions;
using Payment.Application.Responses.Balance;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
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

        public BalanceService(IMapper mapper, IUnitOfWork uof, IUserRestService userRest, 
            ILocationRestService locationRest, ICurrencyExchangeService exchangeService, SqidsEncoder<long> sqids)
        {
            _mapper = mapper;
            _uof = uof;
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
            response.BlockedBalance *= rate;
            response.AvaliableBalance *= rate;

            return response;
        }
    }
}
