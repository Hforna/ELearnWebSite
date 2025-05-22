using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.Api.Binders;
using Progress.Application.UseCases.Interfaces.Progress;

namespace Progress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        [HttpGet("lessons/{lessonId}/complete")]
        public async Task<IActionResult> CompleteUserLesson([FromRoute][ModelBinder(typeof(BinderId))]long lessonId, [FromServices]ICompleteLesson useCase)
        {
            await useCase.Execute(lessonId);

            return Ok();
        }
    }
}
