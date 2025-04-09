using Payment.Application.Requests;
using Payment.Application.Responses.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices.Interfaces
{
    public interface IBalanceService
    {
        public Task<BalanceResponse> GetBalance();
        public Task UserCashOut(CashoutRequest request);
    }
}
