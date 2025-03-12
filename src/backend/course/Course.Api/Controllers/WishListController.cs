using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.UseCases.Repositories.WishLists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/wish-list")]
    public class WishListController : ProjectBaseController
    {
        [AuthenticationUser]
        [HttpGet("add/{courseId}")]
        public async Task<IActionResult> AddCourseToUserWishList([FromRoute][ModelBinder(typeof(BinderId))]long courseId, [FromServices]IAddItemToWishList useCase)
        {
            var result = await useCase.Execute(courseId);

            return Created(string.Empty, result);
        }
    }
}
