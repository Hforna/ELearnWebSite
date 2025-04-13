using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using X.PagedList.Extensions;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "TeacherOnly")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserBalance()
        {
            var result = await _balanceService.GetBalance();

            return Ok(result);
        }

        [HttpPost("create-bank-account")]
        public async Task<IActionResult> CreateUserBankAccount([FromBody]CreateBankAccountRequest request)
        {
            var result = await _balanceService.CreateBankAccount(request);

            return Created(string.Empty, result);
        }

        /// <summary>
        /// Request a cashout for users that wanna transfer their money to bank account.
        /// you can request a cashout only if you have a avaliable balance in account
        /// </summary>
        /// <param name="request">Amount: amount user want to transfer to their account
        /// BankAccountId: the bank account id that user wanna transfer the money</param>
        /// <returns>return a response containing data about the cashout</returns>
        [HttpPost("cashout")]
        public async Task<IActionResult> CashOutAmountFromBalance([FromBody]CashoutRequest request)
        {
            var result = await _balanceService.UserCashOut(request);

            return Created(string.Empty, result);
        }
    }
}
