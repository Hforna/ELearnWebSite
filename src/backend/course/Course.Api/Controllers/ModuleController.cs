using Course.Api.Binders;
using Course.Application.UseCases.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ProjectBaseController
    {
        [Authorize(Policy = "TeacherOnly")]
        [HttpPost("module/{id}")]
        [ProducesResponseType(typeof(ModuleResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateModule([FromRoute][ModelBinder(typeof(BinderId))] long id, [FromBody] CreateModuleRequest request,
        [FromServices] CreateModule useCase)
        {
            var result = await useCase.Execute(request, id);

            return Created(string.Empty, result);
        }
    }
}
