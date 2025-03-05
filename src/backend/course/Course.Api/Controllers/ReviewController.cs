using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Requests;
using Course.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        /// <summary>
        /// create a review of a course,
        /// make a review's text and rate the course
        /// </summary>
        /// <param name="request">fields text and rating for make review</param>
        /// <param name="id">course id that user want to make review</param>
        /// <returns></returns>
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ReviewException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserException), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateReview([FromServices]ICreateReview useCase, 
            [FromBody]CreateReviewRequest request, [FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await useCase.Execute(request, id);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromBody]IDeleteReview useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }
    }
}
