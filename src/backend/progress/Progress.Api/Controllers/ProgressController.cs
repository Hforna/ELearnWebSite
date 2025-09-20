using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.Api.Attributes;
using Progress.Api.Binders;
using Progress.Application.UseCases.Interfaces.Progress;

namespace Progress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UserAuthenticated]
    public class ProgressController : ControllerBase
    {
        [HttpGet("lessons/{lessonId}/complete")]
        public async Task<IActionResult> CompleteUserLesson([FromRoute][ModelBinder(typeof(BinderId))]long lessonId, [FromServices]ICompleteLesson useCase)
        {
            await useCase.Execute(lessonId);

            return Ok();
        }

        
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetUserProgressOnCourse([FromRoute][ModelBinder(typeof(BinderId))]long courseId, [FromServices]ICourseProgress useCase)
        {
            var result = await useCase.Execute(courseId);

            return Ok(result);
        }
    }
}
