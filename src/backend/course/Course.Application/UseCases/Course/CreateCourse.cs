using AutoMapper;
using Course.Application.Services;
using Course.Application.Services.Validators.Course;
using Course.Application.UseCases.Repositories.Course;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
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
    public class CreateCourse : ICreateCourse
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storageService;

        public CreateCourse(IUnitOfWork uof, IMapper mapper, 
            IUserService userService, SqidsEncoder<long> sqids, IStorageService storageService)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
            _storageService = storageService;
        }

        public async Task<CourseResponse> Execute(CreateCourseRequest request)
        {
            Validate(request);

            var userInfos = await _userService.GetUserInfos();

            if (userInfos is null)
                throw new UserException(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);

            var course = _mapper.Map<CourseEntity>(request);
            course.TeacherId = _sqids.Decode(userInfos.id).Single();

            if(request.ThumbnailImage is not null)
            {
                var thumbnail = request.ThumbnailImage.OpenReadStream();

                var imageValidator = ImageValidator.ValidateImage(thumbnail);

                if (imageValidator.isValid)
                {
                    course.Thumbnail = $"{Guid.NewGuid()}{imageValidator.ext}";

                    await _storageService.UploadCourseImage(thumbnail, course.Thumbnail, course.courseIdentifier);
                }
            }

            _uof.courseWrite.AddCourse(course);
            _uof.courseWrite.AddCourseTopics(course.TopicsCovered);

            await _uof.Commit();

            var response = _mapper.Map<CourseResponse>(course);
            response.CourseId = _sqids.Encode(course.Id);
            response.TeacherId = _sqids.Encode(course.Id);

            return response;
        }

        private void Validate(CreateCourseRequest request)
        {
            var validator = new CreateCourseValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new CourseException(errorMessages);
            }
        }
    }
}
