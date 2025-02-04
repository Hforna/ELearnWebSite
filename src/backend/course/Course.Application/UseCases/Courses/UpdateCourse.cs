using AutoMapper;
using Course.Application.Services;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Course
{
    public class UpdateCourse : IUpdateCourse
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqidsEncoder;

        public UpdateCourse(IUnitOfWork uof, IMapper mapper, 
            IStorageService storageService, IUserService userService, 
            SqidsEncoder<long> sqidsEncoder)
        {
            _uof = uof;
            _mapper = mapper;
            _storageService = storageService;
            _userService = userService;
            _sqidsEncoder = sqidsEncoder;
        }

        public async Task<CourseShortResponse> Execute(long id, UpdateCourseRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqidsEncoder.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseByTeacherAndId(userId, id);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER);

            _mapper.Map(request, course);

            if(request.Thumbnail is not null)
            {
                var image = request.Thumbnail.OpenReadStream();

                (bool isValid, string ext) = FileService.ValidateImage(image);

                if(!isValid)
                    throw new CourseException(ResourceExceptMessages.INVALID_FORMAT_IMAGE);

                course.Thumbnail = $"{Guid.NewGuid()}{ext}";

                await _storageService.UploadCourseImage(image, 
                    course.Thumbnail 
                    ,course.courseIdentifier);
            }

            var response = _mapper.Map<CourseShortResponse>(course);
            response.ThumbnailUrl = string.IsNullOrEmpty(course.Thumbnail) ?
                await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail)
                : "";

            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            return response;
        }
    }
}
