using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Quizzes;
using Course.Communication.Requests;
using Course.Exception;
using MassTransit.SagaStateMachine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : ProjectBaseController
    {
        /// <summary>
        /// User create a quiz on a course module
        /// </summary>
        /// <param name="request">fields:
        /// title: title of quiz.
        /// passing score: minimum score user needs to pass on module and get certificate.</param>
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

        /// <summary>
        /// Get a course quiz by course and quiz ids
        /// return a response containing infos about quiz and questions if user set includeQuestions from query as true
        /// </summary>
        /// <param name="quizId">Quiz id as a string</param>
        /// <param name="courseId">Course id as a string</param>
        /// <param name="includeQuestions">set as true will include the questions from quiz</param>
        [HttpGet("{courseId}/{quizId}")]
        [AuthenticationUser]
        [EnableRateLimiting("getQuizLimiter")]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuiz([FromRoute][ModelBinder(typeof(BinderId))] long quizId, 
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId, [FromServices]IGetQuizById useCase, [FromQuery] bool includeQuestions)
        {
            var result = await useCase.Execute(courseId, quizId, includeQuestions);

            return Ok(result);
        }

        /// <summary>
        /// Teacher can delete a quiz from their course
        /// </summary>
        /// <param name="courseId">course id that contains the quiz</param>
        /// <param name="quizId">id of quiz user that wanna delete</param>
        [HttpDelete("{courseId}/{quizId}")]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuizException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz([FromRoute][ModelBinder(typeof(BinderId))]long courseId, 
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId, [FromServices]IDeleteQuiz useCase)
        {
            await useCase.Execute(courseId, quizId);

            return NoContent();
        }

        /// <summary>
        /// Add a question to a user's course quiz
        /// </summary>
        /// <param name="request"></param>
        /// <param name="useCase"></param>
        /// <returns></returns>
        [HttpPost("add-question")]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(QuizException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(QuizException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddQuestionToQuiz([FromBody]CreateQuestionRequest request, [FromServices]ICreateQuestion useCase)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }

        [HttpDelete("{quizId}/question/{questionId}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute][ModelBinder(typeof(BinderId))]long questionId, 
            [FromRoute][ModelBinder(typeof(BinderId))]long quizId, [FromServices]IDeleteQuestion useCase)
        {
            await useCase.Execute(quizId, questionId);

            return NoContent();
        }
    }
}
