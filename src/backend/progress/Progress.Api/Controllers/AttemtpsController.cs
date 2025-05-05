using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.Api.Binders;

namespace Progress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttemtpsController : ControllerBase
    {
        [HttpGet("course/{courseId}/module/{moduleId}/quiz/{quizId}")]
        public async Task<IActionResult> StartQuizAttempt([FromRoute][ModelBinder(typeof(BinderId))]long courseId, 
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId, 
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId)
        {

        }
    }
}
