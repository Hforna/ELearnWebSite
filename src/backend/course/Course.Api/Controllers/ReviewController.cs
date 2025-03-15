using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Reviews;
using Course.Communication.Requests;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(ILogger<ReviewController> logger)
        {
            _logger = logger;
        }


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
            _logger.LogInformation("message sent");
            return Ok(result);
        }

        /// <summary>
        /// Allows a teacher to respond to a review submitted by a user. 
        /// The teacher must be the owner of the course associated with the review to provide an answer.
        /// The response is saved and returned as part of the review.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to which the teacher is responding.</param>
        /// <param name="request">The request object containing the teacher's response to the review.</param>
        /// <param name="useCase">The service responsible for handling the business logic of answering a review.</param>
        /// <returns>
        /// Returns a created response with the created review response object if successful.
        /// </returns>
        [AuthenticationUser]
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost("answer-review/{reviewId}")]
        [ProducesResponseType(typeof(ReviewException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserException), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType()]
        public async Task<IActionResult> AnswerReview([FromRoute][ModelBinder(typeof(BinderId))]long reviewId, [FromBody]CreateReviewResponseRequest request,
            [FromServices]IAnswerReview useCase)
        {
            var result = await useCase.Execute(reviewId, request);

            return Created(string.Empty, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromBody]IDeleteReview useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }
    }
}
