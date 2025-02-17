using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Course.Api.Controllers
{
    [Route("api/course/{courseId}/[controller]")]
    [ApiController]
    public class ModuleController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromRoute][ModelBinder(typeof(BinderId))] long courseId, [FromBody] CreateModuleRequest request,
        [FromServices] CreateModule useCase)
        {
            var result = await useCase.Execute(request, courseId);

            return Created(string.Empty, result);
        }

        [HttpGet(Name = "GetModules")]
        public async Task<IActionResult> GetModules([FromRoute][ModelBinder(typeof(BinderId))] long courseId, [FromServices] IGetModules useCase)
        {
            var result = await useCase.Execute(courseId);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<IActionResult> UpdateModule([FromServices]IUpdateModule useCase,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))] long id,
            [FromBody]UpdateModuleRequest request)
        {
            var result = useCase.Execute(courseId, id, request);

            return Ok(result);
        }

        /// <summary>
        /// Delete a module by its course and id, the module will be deleted in background due to the deletion of the videos.
        /// Users can only delete a module if they own the course.
        /// </summary>
        /// <param name="id">module id</param>
        /// <param name="courseId">course id of module</param>
        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete]
        public async Task<IActionResult> DeleteModule([FromServices] IDeleteModule useCase,
            [FromRoute][ModelBinder(typeof(BinderId))]long id,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId)
        {
            await useCase.Execute(courseId, id);

            return NoContent();
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
        [ProducesResponseType(typeof(ModulesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CourseException), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ModuleException), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeModulePosition([FromRoute]int position, [FromServices]IChangeModulePosition useCase,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await useCase.Execute(courseId, id, position);

            return Ok(result);
        }                  
    }
}
