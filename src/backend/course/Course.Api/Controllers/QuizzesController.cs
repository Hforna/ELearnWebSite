using Course.Api.Attributes;
using Course.Api.Binders;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Exception;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net.Quic;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizzesController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        /// <summary>
        /// User creates a quiz on a course module (Teacher only)
        /// </summary>
        /// <param name="request">Quiz creation request containing:
        /// - title: Title of the quiz
        /// - courseId: ID of the course
        /// - moduleId: ID of the module
        /// - passingScore: Minimum score user needs to pass the module and get certificate</param>
        /// <returns>Returns the created quiz</returns>
        [HttpPost]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
        {
            var result = await _quizService.CreateQuizAsync(request);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get a course quiz by course and quiz ids
        /// Returns a response containing info about quiz and questions if includeQuestions is set to true
        /// </summary>
        /// <param name="quizId">Quiz ID as a string</param>
        /// <param name="courseId">Course ID as a string</param>
        /// <param name="includeQuestions">Set to true to include the questions from the quiz</param>
        /// <returns>Returns quiz details with optional questions</returns>
        [HttpGet("{quizId}")]
        [AuthenticationUser]
        [EnableRateLimiting("getQuizLimiter")]
        [ProducesResponseType(typeof(QuizResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuiz(
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId,
            [FromQuery] bool includeQuestions)
        {
            var result = await _quizService.GetQuizByIdAsync(quizId, includeQuestions);
            return Ok(result);
        }

        /// <summary>
        /// Teacher can delete a quiz from their course
        /// </summary>
        /// <param name="courseId">Course ID that contains the quiz</param>
        /// <param name="quizId">ID of quiz to delete</param>
        [HttpDelete("{quizId}")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz(
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId)
        {
            await _quizService.DeleteQuizAsync(quizId);
            return NoContent();
        }

        /// <summary>
        /// Add a question to a user's course quiz (Teacher only)
        /// </summary>
        /// <param name="request">Question creation request containing quiz ID, question text, and answer options</param>
        /// <returns>Returns the created question with answer options</returns>
        [HttpPost("{quizId}/questions")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(QuestionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddQuestionToQuiz([FromBody] CreateQuestionRequest request, [FromRoute][ModelBinder(typeof(BinderId))]long quizId)
        {
            var result = await _quizService.CreateQuestionAsync(request, quizId);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Delete a question from a quiz (Teacher only)
        /// </summary>
        /// <param name="questionId">Question ID to delete</param>
        /// <param name="quizId">Quiz ID containing the question</param>
        [HttpDelete("{quizId}/question/{questionId}")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuestion(
            [FromRoute][ModelBinder(typeof(BinderId))] long quizId,
            [FromRoute][ModelBinder(typeof(BinderId))] long questionId)
        {
            await _quizService.DeleteQuestionAsync(quizId, questionId);
            return NoContent();
        }
    }
}