using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Consts;
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

        public GetCourse(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IStorageService storage, 
            ICoursesSession coursesSession)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _storage = storage;
            _coursesSession = coursesSession;
        }

        public async Task<CourseResponse> Execute(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var getCoursesList = _coursesSession.GetCoursesVisited() ?? [];

            var courseInList = getCoursesList.Contains(course.Id);

            course.totalVisits +=  courseInList == false ? 1 : 0;
            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            if(!getCoursesList.Contains(course.Id))
                await _coursesSession.AddCourseVisited(course.Id);

            var response = _mapper.Map<CourseResponse>(course);
            response.TeacherProfile = $"{AppUrl.Url}profile/{course.TeacherId}";
            //response.ThumbnailUrl = await _storage.GetCourseImage(course.courseIdentifier, course.Thumbnail);

            return response;
        }
    }
}
