using Course.Api.Attributes;
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

        [HttpPost("user-got-course")]
        [AuthenticationUser]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserGotCourse([FromBody]GetCourseRequest request, [FromServices]IUserGotCourse useCase)
        {
            var result = await useCase.Execute(request);

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
        [HttpGet("teacher-courses/{teacherId}")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)] 
        public async Task<IActionResult> TeacherCourses([FromRoute][ModelBinder(typeof(BinderId))]long teacherId, [FromQuery]int page, 
            [FromQuery]int quantity, [FromServices]ITeacherCourses useCase)
        {
            var result = await useCase.Execute(page, quantity, teacherId);

            return Ok(result);
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
        /// get courses using pagination by user's filter,
        /// return the items quantity that was on query
        /// </summary>
        /// <param name="page">The page of courses</param>
        /// <param name="quantity">quantity of courses for api take</param>
        /// <param name="request">The request contains fields for user filter by course attributes like:
        /// ratings: enum that user can choose a rating with 5 as max note
        /// text: user can search by key words or course titles
        /// course language: enum that user can choose their languages choices
        /// price: user can choose the range of price
        /// </param>
        /// <param name="useCase"></param>
        /// <returns>return courses and information about pagination</returns>
        [HttpPost("filter")]
        [ProducesResponseType(typeof(CoursesPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterCourses([FromQuery] int page, [FromQuery] int quantity, [FromBody] GetCoursesRequest request,
            [FromServices] IGetCourses useCase)
        {
            var result = await useCase.Execute(request, page, quantity);

            return Ok(result);
        }

        /// <summary>
        /// Take courses according pagination that user is registered
        /// </summary>
        /// <param name="page">The page of courses that user bought</param>
        /// <param name="quantity">quantity of courses for api take</param>
        /// <returns>return the courses that user is registered and infos about page</returns>
        [HttpGet("my-courses")]
        [ProducesResponseType(typeof(UserException), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CoursesThatUserBought([FromQuery]int page, [FromQuery]int quantity, [FromServices]ICourseThatUserBought useCase)
        {
            var result = await useCase.Execute(page, quantity);

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
        [HttpGet("ten-courses-visited-week")]
        [ProducesDefaultResponseType()]
        public async Task<IActionResult> GetMostPopularWeekCourses([FromServices] IGetTenMostPopularWeekCourses useCase)
        {
            var result = await useCase.Execute();

            if (result.courses.Any() == false)
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
