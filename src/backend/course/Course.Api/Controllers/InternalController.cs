using Course.Api.Binders;
using Course.Application.InternalServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("InternalServices")]
    public class InternalController : ControllerBase
    {
        private readonly ICourseInternalService _courseInternal;

        public InternalController(ICourseInternalService courseInternal)
        {
            _courseInternal = courseInternal;
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourse([FromRoute][ModelBinder(typeof(BinderId))]long id)
        {
            var result = await _courseInternal.CourseInternalServiceById(id);

            return Ok(result);
        }
    }
}
