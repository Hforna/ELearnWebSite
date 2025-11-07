using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Communication.Enums;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Exception;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Org.BouncyCastle.Asn1.Cms;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Creates a new course (Teacher only)
        /// </summary>
        [Authorize(Policy = "TeacherOnly")]
        [EnableRateLimiting("createCourseLimiter")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseShortResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseRequest request)
        {
            var result = await _courseService.CreateCourse(request);
            return Created(string.Empty, result);
        }

        [HttpPut("{courseId}/publish")]
        [AuthenticationUser]
        public async Task<IActionResult> PublishCourse([FromRoute][ModelBinder(typeof(BinderId))]long courseId)
        {
            await _courseService.PublishCourse(courseId);

            return NoContent();
        }

        /// <summary>
        /// Check if user has access to a specific course
        /// </summary>
        [HttpPost("user-got-course")]
        [AuthenticationUser]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserGotCourse([FromBody] GetCourseRequest request)
        {
            var result = await _courseService.UserGotCourse(request);
            return Ok(result);
        }

        /// <summary>
        /// Get the total number of lessons in a course
        /// </summary>
        [HttpGet("{id}/count")]
        public async Task<IActionResult> GetCourseLessonsCount([FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _courseService.CourseLessonsCount(id);
            return Ok(result);
        }

        /// <summary>
        /// Get active courses for a specific teacher by their teacher ID.
        /// </summary>
        /// <param name="teacherId">The unique identifier of the teacher.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="quantity">The number of courses to return per page.</param>
        /// <returns>A paginated list of active courses for the specified teacher.</returns>
        /// <response code="200">Returns the paginated list of active courses.</response>
        /// <response code="404">If no courses are found for the given teacher or if the teacher does not exist.</response>
        [HttpGet("teachers/{teacherId}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TeacherCourses(
            [FromRoute][ModelBinder(typeof(BinderId))] long teacherId,
            [FromQuery] int page,
            [FromQuery] int quantity)
        {
            var result = await _courseService.TeacherCourses(page, quantity, teacherId);
            return Ok(result);
        }

        /// <summary>
        /// Update an existing course (Teacher only)
        /// </summary>
        [Authorize(Policy = "TeacherOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(
            [FromForm] UpdateCourseRequest request,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _courseService.UpdateCourse(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Get courses using pagination by user's filter,
        /// return the items quantity that was on query
        /// </summary>
        /// <param name="page">The page of courses</param>
        /// <param name="quantity">quantity of courses for api take</param>
        /// <returns>return courses and information about pagination</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(CoursesPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterCourses(
            [FromQuery] int page,
            [FromQuery] int quantity,
            [FromQuery]PriceEnum? priceEnum, [FromQuery]string? text, [FromQuery]List<CourseRatingEnum>? courseRating, [FromQuery]List<LanguagesEnum>? languages)
        {
            var request = new GetCoursesRequest()
            {
                Price = priceEnum,
                Text = text,
                Ratings = courseRating,
                Languages = languages
            };
            var result = await _courseService.GetCourses(request, page, quantity);
            return Ok(result);
        }

        /// <summary>
        /// Take courses according pagination that user is registered
        /// </summary>
        /// <param name="page">The page of courses that user bought</param>
        /// <param name="quantity">quantity of courses for api take</param>
        /// <returns>return the courses that user is registered and infos about page</returns>
        [HttpGet("mine")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CoursesThatUserBought([FromQuery] int page, [FromQuery] int quantity)
        {
            var result = await _courseService.CoursesUserBought(page, quantity);
            return Ok(result);
        }

        /// <summary>
        /// Returns the ten most visited courses of the week.
        /// Get most popular courses using cache
        /// </summary>
        /// <returns>
        /// Returns a 200 (OK) status with the list of the most popular courses of the week,  
        /// or 204 (No Content) if no courses are found.
        /// </returns>
        [HttpGet("most-visited-week")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetMostPopularWeekCourses()
        {
            var result = await _courseService.GetTenMostPopularWeekCourses();

            if (result is null || result.courses.Any() == false)
                return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Get a specific course by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse([FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _courseService.GetCourse(id);
            return Ok(result);
        }

        /// <summary>
        /// Delete a course (Teacher only)
        /// </summary>
        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteCourse([FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            await _courseService.DeleteCourse(id);
            return NoContent();
        }
    }
}