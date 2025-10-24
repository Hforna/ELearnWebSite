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
    [Route("api/course/{courseId}/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        /// <summary>
        /// Create a new module for a course (Teacher only)
        /// </summary>
        /// <param name="courseId">The course ID</param>
        /// <param name="request">The module creation request</param>
        /// <returns>Returns the created module with HATEOAS links</returns>
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ModuleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateModule(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromBody] CreateModuleRequest request)
        {
            var result = await _moduleService.CreateModuleAsync(request, courseId);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Get all modules for a specific course
        /// </summary>
        /// <param name="courseId">The course ID</param>
        /// <returns>Returns all modules with HATEOAS links</returns>
        [HttpGet(Name = "GetModules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetModules(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId)
        {
            var result = await _moduleService.GetModulesAsync(courseId);
            return Ok(result);
        }

        /// <summary>
        /// Update an existing module (Teacher only)
        /// </summary>
        /// <param name="courseId">The course ID</param>
        /// <param name="id">The module ID</param>
        /// <param name="request">The module update request</param>
        /// <returns>Returns the updated module</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateModule(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id,
            [FromBody] UpdateModuleRequest request)
        {
            var result = await _moduleService.UpdateModuleAsync(courseId, id, request);
            return Ok(result);
        }

        /// <summary>
        /// Changes the position of a module within a course.
        /// Users can only change a module's position if they own the course.
        /// </summary>
        /// <param name="position">The new position for the module within the course.</param>
        /// <param name="courseId">The id of the course containing the module.</param>
        /// <param name="id">The id of the module to be repositioned.</param>
        /// <returns>Returns the updated list of modules within the course, including the course Id.</returns>
        [HttpPut("{id}/position/{position}")]
        [Authorize(Policy = "TeacherOnly")]
        [ProducesResponseType(typeof(ModulesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeModulePosition(
            [FromRoute] int position,
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            var result = await _moduleService.ChangeModulePositionAsync(courseId, id, position);
            return Ok(result);
        }

        /// <summary>
        /// Delete a module by its course and id, the module will be soft deleted.
        /// Users can only delete a module if they own the course.
        /// </summary>
        /// <param name="id">Module ID</param>
        /// <param name="courseId">Course ID of module</param>
        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteModule(
            [FromRoute][ModelBinder(typeof(BinderId))] long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id)
        {
            await _moduleService.DeleteModuleAsync(courseId, id);
            return NoContent();
        }
    }
}