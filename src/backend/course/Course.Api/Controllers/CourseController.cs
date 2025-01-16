using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    public class CourseController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseShortResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromServices] ICreateCourse useCase, [FromForm] CreateCourseRequest request)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(CoursesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterCourses([FromQuery] int page, [FromQuery] int items, [FromBody] GetCoursesRequest request,
            [FromServices] IGetCourses useCase)
        {
            var result = await useCase.Execute(request, page, items);

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
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromServices]IDeleteCourse useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }
    }
}
