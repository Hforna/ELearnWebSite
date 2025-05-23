﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Payment.Api.Attributes;
using Payment.Api.Binders;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Exceptions;
using System.Net;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticationUser]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;


        /// <summary>
        /// process user purchase by pix, a brazilian method for process payment.
        /// user's courses will be allowed after he make the purchase on their account
        /// </summary>
        /// <param name="request">infos about account that user will make the purchase</param>
        /// <returns>hash: contains the key that user will need to make the purchase.
        /// qrcode: the qr code in text format for user make the purchase
        /// expired on: user will has a specific time to make the purchase, otherwise the transaction will fail
        /// </returns>
        [HttpPost("pix")]
        [EnableRateLimiting("PixPayment")]
        public async Task<IActionResult> ProcessPixPayment([FromBody]PixPaymentRequest request)
        {
            var result = await _paymentService.ProcessPixPayment(request);

            return Ok(result);
        }

        /// <summary>
        /// make the purchase of user order by card.
        /// user can choose if wanna pay on credit card or debt card choosing the installments that they prefer
        /// </summary>
        /// <param name="request"></param>
        /// <returns>return the response containing infos about transaction</returns>
        [EnableRateLimiting("CardPayment")]
        [HttpPost("card")]
        public async Task<IActionResult> ProcessCardPayment([FromBody]CardPaymentRequest request)
        {
            var result = await _paymentService.ProcessCardPayment(request);

            return Ok(result);
        }

        /// <summary>
        /// Request a refund of some course that user has.
        /// </summary>
        /// <param name="courseId">course id which user wanna get refund</param>
        /// <returns>Return infos about refund transaction</returns>
        [HttpGet("refund")]
        [Route("{courseId}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(OrderException), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(RestException), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RequestCourseRefund([FromRoute][ModelBinder(typeof(BinderId))]long courseId)
        {
            var result = await _paymentService.RequestCourseRefund(courseId);

            return Ok(result);
        }
    }
}
