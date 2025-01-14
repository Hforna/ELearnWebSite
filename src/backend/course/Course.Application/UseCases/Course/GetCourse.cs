using AutoMapper;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
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

        public GetCourse(IUnitOfWork uof, IMapper mapper, SqidsEncoder<long> sqids, IStorageService storage)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _storage = storage;
        }

        public async Task<CourseResponse> Execute(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var response = _mapper.Map<CourseResponse>(course);
            response.ThumbnailUrl = await _storage.GetCourseImage(course.courseIdentifier, course.Thumbnail);

            return response;
        }
    }
}
