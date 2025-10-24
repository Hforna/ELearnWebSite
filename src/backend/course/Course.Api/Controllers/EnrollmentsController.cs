using Course.Api.Binders;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Get course's enrollments for course owner (Teacher only)
        /// </summary>
        /// <param name="page">Enrollment page number</param>
        /// <param name="quantity">Quantity of items to retrieve</param>
        /// <param name="courseId">The course ID</param>
        /// <returns>Returns a list of enrollments and info about pagination</returns>
        [Authorize(Policy = "TeacherOnly")]
        [HttpGet("course/{courseId}")]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCourseEnrollments(
            [FromQuery] int page,
            [FromQuery] int quantity,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await _enrollmentService.GetCourseEnrollments(courseId, page, quantity);

            if (!result.Enrollments.Any())
                return NoContent();

            return Ok(result);
        }
    }
}