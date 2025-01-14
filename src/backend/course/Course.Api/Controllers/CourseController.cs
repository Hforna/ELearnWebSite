using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    public class CourseController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromServices]ICreateCourse useCase, [FromForm]CreateCourseRequest request)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }

        [HttpPost("get-courses")]
        [ProducesResponseType(typeof(CoursesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourses([FromQuery]int page, [FromQuery]int items, [FromBody]GetCoursesRequest request, 
            [FromServices]IGetCourses useCase)
        {
            var result = await useCase.Execute(request, page, items);

            return Ok(result);
        }

    }
}
