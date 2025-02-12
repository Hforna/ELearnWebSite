using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Lessons;
using Course.Communication.Requests;
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
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "TeacherOnly")]
        public async Task<IActionResult> CreateLesson([FromServices]ICreateLesson useCase, [FromForm]CreateLessonRequest request,
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
        [HttpGet("lessons", Name = "GetLessons")]
        public async Task<IActionResult> GetLessons([FromServices]IGetLessons useCase, 
            [FromRoute][ModelBinder(typeof(BinderId))]long moduleId,
            [FromRoute][ModelBinder(typeof(BinderId))]long courseId)
        {
            var result = await useCase.Execute(moduleId);

            return Ok(result);
        }
    }
}
