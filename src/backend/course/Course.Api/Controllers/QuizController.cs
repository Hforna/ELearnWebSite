using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Quizzes;
using Course.Communication.Requests;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : ProjectBaseController
    {
        [HttpPost("create-quiz")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(CourseException), (int)System.Net.HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(CourseException), (int)System.Net.HttpStatusCode.NotFound)]
        [ProducesDefaultResponseType()]
        public async Task<IActionResult> CreateQuiz([FromBody]CreateQuizRequest request, [FromServices]ICreateQuiz useCase)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }
    }
}
