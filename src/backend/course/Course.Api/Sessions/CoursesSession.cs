using Course.Domain.Repositories;
using Course.Domain.Sessions;
using Course.Exception;
using System.Text.Json;

namespace Course.Api.Sessions
{
    public class CoursesSession : ICoursesSession
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUnitOfWork _uof;

        public CoursesSession(IHttpContextAccessor httpContext, IUnitOfWork uof)
        {
            _httpContext = httpContext;
            _uof = uof;
        }

        public List<long>? GetCoursesVisited()
        {
            var session = _httpContext.HttpContext!.Session;

            return session.TryGetValue("coursesVisited", out var value) 
                ? JsonSerializer.Deserialize<List<long>>(value) : null;
        }

        public async Task AddCourseVisited(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var session = _httpContext.HttpContext!.Session;

            if(session.TryGetValue("coursesVisited", out var value))
            {
                var deserializeValue = JsonSerializer.Deserialize<string>(value);

                var valueAsList = deserializeValue!.Split(",").ToList();

                valueAsList.Add($"{courseId}, ");

                var serializeList = JsonSerializer.Serialize(valueAsList);

                session.SetString("coursesVisited", serializeList);
            } else
            {
                session.SetString("coursesVisited", $"{courseId}, ");
            }
        }
    }
}
