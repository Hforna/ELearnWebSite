using Course.Api.Binders;
using Course.Application.UseCases.Course;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    public class CourseController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [EnableRateLimiting("createCourseLimiter")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseShortResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromServices] ICreateCourse useCase, [FromForm] CreateCourseRequest request)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateCourse([FromServices]UpdateCourse useCase, 
            [FromForm]UpdateCourseRequest request, [FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await useCase.Execute(id, request);

            return Ok(result);
        }


        /// <summary>
        /// Returns the ten most visited courses of the week.
        /// Enpoint get courses using cache
        /// </summary>
        /// <returns>
        /// Returns a 200 (OK) status with the list of the most popular courses of the week,  
        /// or 204 (No Content) if no courses are found.
        /// </returns>
        [HttpGet("ten-courses-visited-week")]
        public async Task<IActionResult> GetMostPopularWeekCourses([FromServices]IGetTenMostPopularWeekCourses useCase)
        {
            var result = await useCase.Execute();

            if (result.courses.Any() == false)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(CoursesPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterCourses([FromQuery] int page, [FromQuery] int items, [FromBody] GetCoursesRequest request,
            [FromServices] IGetCourses useCase)
        {
            var result = await useCase.Execute(request, page, items);

            return Ok(result);
        }

        /// <summary>
        /// Returns the five most visited courses of the week.
        /// Get most popular courses using cache
        /// </summary>
        /// <returns>
        /// Returns a 200 (OK) status with the list of the most popular courses of the week,  
        /// or 204 (No Content) if no courses are found.
        /// </returns>
        [HttpGet("five-courses-visited-week")]
        [ProducesDefaultResponseType()]
        public async Task<IActionResult> GetMostPopularWeekCourses([FromServices] IGetTenMostPopularWeekCourses useCase)
        {
            var result = await useCase.Execute();

            if (result.Courses.Any() == false)
                return NoContent();
            return Ok(result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourse([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromServices]IGetCourse useCase)
        {
            var result = await useCase.Execute(id);

            return Ok(result);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteCourse([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromServices]IDeleteCourse useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }
    }
}
