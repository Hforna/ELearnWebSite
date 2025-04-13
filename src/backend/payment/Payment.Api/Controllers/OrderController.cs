using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Attributes;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Exceptions;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        /// <summary>
        /// add a course to user order, user must be logged to add a course to their order
        /// </summary>
        /// <param name="request">a request body that contain the course id that user wanna add on order</param>
        /// <returns>return the orderItem that contain: order item id, course id, user id and course price</returns>
        [HttpPost]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(OrderException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCourseToOrder([FromBody]AddCourseToOrderRequest request)
        {
            var result = await _orderService.AddCourseToOrder(request);

            return Created(string.Empty, result);
        }

        /// <summary>
        /// get the order of user, users just can get if them is logged
        /// </summary>
        /// <returns>return the order of user and order items in order</returns>
        [HttpGet]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(OrderException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserOrder()
        {
            var result = await _orderService.GetUserOrder();

            return Ok(result);
        }

        [HttpGet("order-history")]
        [AuthenticationUser]
        [ProducesResponseType(typeof(OrderException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OrderHistory([FromQuery]int page, [FromQuery]int quantity)
        {
            var result = await _orderService.GetOrderHistory(page, quantity);

            if (result.Orders is null || !result.Orders.Any())
                return NoContent();

            return Ok(result);
        }
    }
}
