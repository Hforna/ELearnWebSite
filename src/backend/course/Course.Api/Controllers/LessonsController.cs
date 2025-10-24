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

namespace Course.Api.Controllers
{
    [Route("api/course/{courseId}/module/{moduleId}/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        /// <summary>
        /// Create a lesson for a specific module, user can create a lesson if created a account as teacher
        /// and if user is course's owner.
        /// Lesson's video must be mp4 otherwise it will be transcoded for mp4
        /// </summary>
        /// <param name="moduleId">Module ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="request">The lesson creation request</param>
        [HttpPost]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLesson(
            [FromForm] CreateLessonRequest request,
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await _lessonService.CreateLessonAsync(request, moduleId, courseId);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get all lessons from module
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="moduleId">Module ID</param>
        /// <returns>Return all lessons of module and its module id</returns>
        [HttpGet(Name = "GetLessons")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetLessons(
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await _lessonService.GetLessonsAsync(moduleId, courseId);
            return Ok(result);
        }

        /// <summary>
        /// Get Lesson By lesson id.
        /// The user can only access if the lesson is free or the user has course access.
        /// </summary>
        /// <param name="moduleId">Module ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="id">Lesson ID</param>
        /// <returns>Return lesson infos and video url</returns>
        [HttpGet("{id}", Name = "GetLesson")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetLesson(
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _lessonService.GetLessonAsync(courseId, moduleId, id);
            return Ok(result);
        }

        /// <summary>
        /// Get short information about a lesson
        /// </summary>
        /// <param name="lessonId">Lesson ID</param>
        /// <returns>Return basic lesson information</returns>
        [HttpGet("{lessonId}/lessons-infos")]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetLessonShortInfos(
            [FromRoute][ModelBinder(typeof(BinderId))] long lessonId)
        {
            var result = await _lessonService.GetLessonShortInfosAsync(lessonId);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing lesson within a module and course.
        /// </summary>
        /// <remarks>
        /// This endpoint allows teachers to update a lesson's details, including its title, order, and associated module.
        /// The lesson's ID, module ID, and course ID are required to identify the lesson to be updated.
        /// </remarks>
        /// <param name="moduleId">The encoded ID of the module to which the lesson belongs.</param>
        /// <param name="courseId">The encoded ID of the course to which the module belongs.</param>
        /// <param name="id">The encoded ID of the lesson to be updated.</param>
        /// <param name="request">The payload containing the updated lesson details.</param>
        /// <returns>Returns the updated lesson details if the operation is successful.</returns>
        /// <response code="200">Returns the updated lesson details.</response>
        /// <response code="400">If the request is invalid (e.g., invalid module ID, order out of range).</response>
        /// <response code="401">If the user is not authorized to update the lesson.</response>
        /// <response code="404">If the lesson, module, or course does not exist.</response>
        [Authorize(Policy = "TeacherOnly")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLesson(
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id,
            [FromBody] UpdateLessonRequest request)
        {
            var result = await _lessonService.UpdateLessonAsync(courseId, moduleId, id, request);
            return Ok(result);
        }

        /// <summary>
        /// Delete a lesson by id.
        /// Delete lesson's video on azure storage service,
        /// only users that own the lesson's course can delete it
        /// </summary>
        /// <param name="moduleId">Lesson module ID</param>
        /// <param name="courseId">Lesson course ID</param>
        /// <param name="id">Lesson ID</param>
        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLesson(
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            await _lessonService.DeleteLessonAsync(courseId, moduleId, id);
            return NoContent();
        }
    }
}