using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.Api.Binders;
using Progress.Application.Requests;
using Progress.Application.UseCases.Interfaces;

namespace Progress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttemtpsController : ControllerBase
    {
        [HttpGet("course/{courseId}/quiz/{quizId}/start")]
        public async Task<IActionResult> StartQuizAttempt([FromRoute][ModelBinder(typeof(BinderId))]long courseId, 
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId, [FromServices]IStartQuizAttempt useCase)
        {
            var result = await useCase.Execute(courseId, quizId);

            return Created(string.Empty, result);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuizAnswers([FromBody]SubmitQuizRequest request, [FromServices]ISubmitQuizAttempt useCase)
        {
            var result = await useCase.Execute(request);

            return Ok(result);
        }
    }
}
