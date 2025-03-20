using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Domain.Sessions;
using Course.Exception;
using Microsoft.Extensions.Configuration;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Course
{
    public class GetCourse : IGetCourse 
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storage;
        private readonly ICoursesSession _coursesSession;
        private readonly ILinkService _linkService;
        private readonly ICourseCache _courseCache;

        public GetCourse(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IStorageService storage, 
            ICoursesSession coursesSession, ILinkService linkService, ICourseCache courseCache)
        {
            _uof = uof;
            _linkService = linkService;
            _mapper = mapper;
            _courseCache = courseCache;
            _sqids = sqids;
            _storage = storage;
            _coursesSession = coursesSession;
        }

        public async Task<CourseResponse> Execute(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            var getCourseTest = await _courseCache.GetMostPopularCourses();

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, HttpStatusCode.NotFound);

            var getCoursesList = _coursesSession.GetCoursesVisited() ?? [];

            var courseInList = getCoursesList.Contains(course.Id);

            if (!courseInList)
            {
                await _courseCache.SetCourseOnMostVisited(course.Id);
                await _coursesSession.AddCourseVisited(course.Id);
            }

            course.totalVisits += courseInList == false ? 1 : 0;
            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            var response = _mapper.Map<CourseResponse>(course);
            response.AddLink("modules", _linkService.GenerateResourceLink("GetModules", new { courseId = _sqids.Encode(course.Id),
                id = _sqids.Encode(course.Id) }), "GET");
            response.TeacherProfile = $"https://localhost:8080/profile/{course.TeacherId}";
            //response.ThumbnailUrl = await _storage.GetCourseImage(course.courseIdentifier, course.Thumbnail);

            return response;
        }
    }
}
