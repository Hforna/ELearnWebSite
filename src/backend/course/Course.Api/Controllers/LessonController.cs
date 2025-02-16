using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/course/{courseId}/module/{moduleId}/[controller]")]
    public class LessonController : ProjectBaseController
    {
        /// <summary>
        /// Create a lesson for a specific module, user can create a lesson if created a account as teacher
        /// and if user is course's owner.
        /// Lesson's video must be mp4 otherwise it will be transcoded for mp4
        /// </summary>
        /// <param name="moduleId">module id</param>
        /// <param name="courseId">course id</param>
        [HttpPost]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(typeof(ModuleException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LessonException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(VideoException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateLesson([FromServices] ICreateLesson useCase, [FromForm] CreateLessonRequest request,
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await useCase.Execute(request, moduleId, courseId);

            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get all lessons from module
        /// </summary>
        /// <returns>
        /// return all lessons of module and its module id
        /// </returns>
        /// <param name="courseId">course id</param>
        /// <param name="moduleId">module id</param>
        [HttpGet(Name = "GetLessons")]
        [ProducesResponseType(typeof(LessonException), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetLessons([FromServices] IGetLessons useCase,
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await useCase.Execute(moduleId, courseId);

            return Ok(result);
        }


        /// <summary>
        /// Get Lesson By lesson id.
        /// The user can only access if the lesson is free or the user has course access.
        /// </summary>
        /// <param name="moduleId">module id</param>
        /// <param name="courseId">course id</param>
        /// <param name="id"></param>
        /// <returns>return lesson infos and video url</returns>
        [HttpGet("{id}", Name = "GetLesson")]
        [ProducesResponseType(typeof(LessonException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LessonException), StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetLesson([FromServices] IGetLesson useCase,
            [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await useCase.Execute(courseId, moduleId, id);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing lesson within a module and course.
        /// </summary>
        /// <remarks>
        /// This endpoint allows teachers to update a lesson's details, including its title, order, and associated module.
        /// The lesson's ID, module ID, and course ID are required to identify the lesson to be updated.
        /// </remarks>
        /// <param name="useCase">The use case responsible for handling the lesson update logic.</param>
        /// <param name="moduleId">The encoded ID of the module to which the lesson belongs.</param>
        /// <param name="courseId">The encoded ID of the course to which the module belongs.</param>
        /// <param name="id">The encoded ID of the lesson to be updated.</param>
        /// <param name="request">The payload containing the updated lesson details.</param>
        /// <returns>
        /// Returns the updated lesson details if the operation is successful.
        /// </returns>
        /// <response code="200">Returns the updated lesson details.</response>
        /// <response code="400">If the request is invalid (e.g., invalid module ID, order out of range).</response>
        /// <response code="401">If the user is not authorized to update the lesson.</response>
        /// <response code="404">If the lesson, module, or course does not exist.</response>
        [Authorize(Policy = "TeacherOnly")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LessonResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLesson([FromServices]IUpdateLesson useCase, [FromRoute][ModelBinder(typeof(BinderId))] long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id,
            [FromBody]UpdateLessonRequest request)
        {
            var result = await useCase.Execute(courseId, moduleId, id, request);

            return Ok(result);
        }

        /// <summary>
        /// Delete a lesson by id.
        /// delete lesson's video on azure storage service,
        /// only users that own the lesson's course can delete it
        /// </summary>
        /// <param name="moduleId">lesson module id</param>
        /// <param name="courseId">lesson course id</param>
        /// <param name="id">lesson id</param>
        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(LessonException), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLesson([FromServices] IDeleteLesson useCase,
            [FromRoute][ModelBinder(typeof(BinderId))]long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            await useCase.Execute(courseId, moduleId, id);

            return NoContent();
        }
    }
}
