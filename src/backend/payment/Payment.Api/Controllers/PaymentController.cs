using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

        [HttpPost("pix")]
        public async Task<IActionResult> ProcessPixPayment([FromBody]PixPaymentRequest request)
        {
            var result = await _paymentService.ProcessPixPayment(request);

            return Ok(result);
        }
    }
}
