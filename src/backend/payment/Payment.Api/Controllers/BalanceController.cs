using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBalance()
        {
            var result = await _balanceService.GetBalance();

            return Ok(result);
        }

        [HttpPost("CashOut")]
        public async Task<IActionResult> CashOutAmountFromBalance([FromBody]CashoutRequest request)
        {

        }
    }
}
