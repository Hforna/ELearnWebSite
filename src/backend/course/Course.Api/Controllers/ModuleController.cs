using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
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

        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete]
        public async Task<IActionResult> DeleteModule([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromServices]IDeleteModule useCase)
        {
            await useCase.Execute(id);

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
        public async Task<IActionResult> ChangeModulePosition([FromRoute]int position, [FromServices]IChangeModulePosition useCase,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId,
            [FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await useCase.Execute(courseId, id, position);

            return Ok(result);
        }
            

        
    }
}
