using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/course/{id}/[controller]")]
    [ApiController]
    public class ModuleController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromRoute][ModelBinder(typeof(BinderId))] long id, [FromBody] CreateModuleRequest request,
        [FromServices] CreateModule useCase)
        {
            var result = await useCase.Execute(request, id);

            return Created(string.Empty, result);
        }

        [HttpGet(Name = "GetModules")]
        public async Task<IActionResult> GetModules([FromRoute][ModelBinder(typeof(BinderId))] long id, [FromServices] IGetModules useCase)
        {
            var result = await useCase.Execute(id);

            return Ok(result);
        }

        [Authorize(Policy = "TeacherOnly")]
        [HttpDelete]
        public async Task<IActionResult> DeleteModule([FromRoute][ModelBinder(typeof(BinderId))]long id, [FromServices]IDeleteModule useCase)
        {
            await useCase.Execute(id);

            return NoContent();
        }

        
    }
}
