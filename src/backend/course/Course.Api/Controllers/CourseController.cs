using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    public class CourseController : ProjectBaseController
    {
        //[Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromServices]ICreateCourse useCase, [FromForm]CreateCourseRequest request)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }
    }
}
