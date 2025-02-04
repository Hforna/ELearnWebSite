using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    public class LessonController : ProjectBaseController
    {
        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> CreateLesson([FromServices]ICreateLesson useCase, [FromForm]CreateLessonRequest request, 
            [FromBody][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await useCase.Execute(request, id);

            return Created(string.Empty, result);
        }
    }
}
