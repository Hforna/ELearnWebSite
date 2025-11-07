using Course.Api.Binders;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Communication.Responses;
using Course.Exception;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/wish-list")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        /// <summary>
        /// Add a course to user wish list.
        /// If user is not authenticated, the wish list will be saved in cache.
        /// </summary>
        /// <param name="courseId">Course ID to add to wish list</param>
        /// <returns>Returns wishlist ID and course ID</returns>
        [HttpPost("{courseId}")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AddCourseToUserWishList(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var sessionId = HttpContext.Session.Id;
            var result = await _wishListService.AddItemToWishList(courseId, sessionId);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get the user's wish list.
        /// If user is not authenticated, retrieves wish list from cache.
        /// </summary>
        /// <returns>Returns the user's wish list with courses</returns>
        [HttpGet]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetUserWishList()
        {
            var sessionId = HttpContext.Session.Id;
            var result = await _wishListService.GetUserWishList(sessionId);

            if (result.Courses is null || result.Courses.Any() == false)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Delete a course from user wish list.
        /// If user isn't logged in, it will be deleted from cache.
        /// </summary>
        /// <param name="courseId">Course ID to remove from wish list</param>
        [HttpDelete("{courseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveCourseFromUserWishList(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var sessionId = HttpContext.Session.Id;
            await _wishListService.RemoveCourseFromWishList(courseId, sessionId);
            return NoContent();
        }
    }
}