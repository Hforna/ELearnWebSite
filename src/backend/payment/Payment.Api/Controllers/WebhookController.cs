using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Exceptions;
using Stripe;
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

        [HttpPost]
        [Route("stripe-api")]
        public async Task<IActionResult> CardStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var formatBody = EventUtility.ParseEvent(json, false);
                var typeOfPayload = formatBody.Type.Split(".")[0];

                switch(typeOfPayload)
                {
                    case "payment_intent":
                        await _webhookService.CardStripeWebhook(formatBody);
                        break;
                }

                return Ok();
            }catch(StripeException ex)
            {
                return BadRequest();
            }
        }
    }
}
