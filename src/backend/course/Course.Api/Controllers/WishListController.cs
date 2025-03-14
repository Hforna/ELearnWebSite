﻿using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.UseCases.Repositories.WishLists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/wish-list")]
    public class WishListController : ProjectBaseController
    {
        /// <summary>
        /// Add a course to user wish list, if user is not autheticated the wish list will be saved on cache
        /// </summary>
        /// <param name="courseId">course id of course that user wanna add on wish list</param>
        /// <returns>return wishlist id and course id</returns>
        [HttpGet("add/{courseId}")]
        public async Task<IActionResult> AddCourseToUserWishList([FromRoute][ModelBinder(typeof(BinderId))]long courseId, [FromServices]IAddItemToWishList useCase)
        {
            var sessionId = HttpContext.Session.Id;
            var result = await useCase.Execute(courseId, sessionId);

            return Created(string.Empty, result);
        }
    }
}
