using Course.Api.Binders;
using Course.Application.UseCases.Repositories.Enrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    public class EnrollmentController : ProjectBaseController
    {
        /// <summary>
        /// get course's enrollments for user owner
        /// </summary>
        /// <param name="page">enrollment page number</param>
        /// <param name="quantity">quantity of items for take</param>
        /// <param name="courseId">enrollment's course id</param>
        /// <returns>return a list of enrollments and infos about pagination</returns>
        [Authorize(Policy = "TeacherOnly")]
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseEnrollments([FromQuery]int page, [FromQuery]int quantity, [FromRoute][ModelBinder(typeof(BinderId))]long courseId, 
            [FromBody]IGetCourseEnrollments useCase)
        {
            var result = await useCase.Execute(courseId, page, quantity);

            if (!result.Enrollments.Any())
                return NoContent();
            return Ok(result);
        }
    }
}
