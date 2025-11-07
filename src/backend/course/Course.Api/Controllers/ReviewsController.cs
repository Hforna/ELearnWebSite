using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Exception;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Create a review for a course.
        /// Make a review's text and rate the course.
        /// </summary>
        /// <param name="request">Request containing review text and rating</param>
        /// <param name="id">Course ID that user wants to review</param>
        /// <returns>Returns the created review</returns>
        [HttpPost("{id}")]
        [AuthenticationUser]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateReview(
            [FromBody] CreateReviewRequest request,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _reviewService.CreateReview(request, id);
            _logger.LogInformation("Review created for course {CourseId}", id);
            return Ok(result);
        }

        /// <summary>
        /// Allows a teacher to respond to a review submitted by a user. 
        /// The teacher must be the owner of the course associated with the review to provide an answer.
        /// The response is saved and returned as part of the review.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review to which the teacher is responding.</param>
        /// <param name="request">The request object containing the teacher's response to the review.</param>
        /// <returns>Returns the created review response object if successful.</returns>
        [AuthenticationUser]
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost("answer-review/{reviewId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AnswerReview(
            [FromRoute][ModelBinder(typeof(BinderId))] long reviewId,
            [FromBody] CreateReviewResponseRequest request)
        {
            var result = await _reviewService.AnswerReview(reviewId, request);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get reviews of a course
        /// </summary>
        /// <param name="courseId">The course ID to get reviews for</param>
        /// <returns>Returns a list of the course's reviews</returns>
        [HttpGet("{courseId}/reviews")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCourseReviews(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await _reviewService.GetReviews(courseId);

            if (result.Reviews.Count < 1)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Get a specific review by ID
        /// </summary>
        /// <param name="id">Review ID</param>
        /// <returns>Returns the review details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetReview(
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _reviewService.GetReview(id);
            return Ok(result);
        }

        /// <summary>
        /// Delete a review by ID
        /// </summary>
        /// <param name="id">Review ID to delete</param>
        [HttpDelete("{id}")]
        [AuthenticationUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteReview(
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            await _reviewService.DeleteReview(id);
            return NoContent();
        }
    }
}