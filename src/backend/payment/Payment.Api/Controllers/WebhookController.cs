using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Exceptions;
using System.Text.Json;

namespace Payment.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebhookService _webhookService;

        public WebhookController(ILogger<WebhookController> logger, IConfiguration configuration, IWebhookService webhookService)
        {
            _logger = logger;
            _webhookService = webhookService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("trio-api")]
        public async Task<IActionResult> PixTrioWebhook()
        {
            string? reqBody;
            using(var stream = new StreamReader(Request.Body))
            {
                reqBody = await stream.ReadToEndAsync();
            }

            var requestSignature = Request.Headers["X-Signature"].ToString();
            var appSignature = _configuration.GetValue<string>("app:trioSignature");

            if (requestSignature is null || appSignature != requestSignature)
                throw new RestException("Invalid or empty signature", System.Net.HttpStatusCode.Unauthorized);

            var deserialize = JsonSerializer.Deserialize<TrioWebhookPayload>(reqBody);

            await _webhookService.PixTrioWebhook(deserialize);

            return Ok();
        }

        //[HttpPost]
        //[Route("stripe-api")]
        //public async Task<IActionResult> CardStripeWebhook()
        //{
        //
        //}
    }
}
