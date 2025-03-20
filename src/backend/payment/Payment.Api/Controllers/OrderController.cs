using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;

namespace Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) => _orderService = orderService;

        [HttpPost]
        public async Task<IActionResult> AddCourseToOrder([FromBody]AddCourseToOrderRequest request)
        {
            var result = await _orderService.AddCourseToOrder(request);

            return Created(string.Empty, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrder()
        {
            var result = await _orderService.GetUserOrder();

            return Ok(result);
        }
    }
}
