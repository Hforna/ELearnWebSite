using AutoMapper;
using Course.Application.Services.Validators.Module;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using MediaInfo.DotNetWrapper;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Modules
{
    public class UpdateModule : IUpdateModule
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public UpdateModule(IUnitOfWork uof, IMapper mapper, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<ModuleResponse> Execute(long courseId, long moduleId, UpdateModuleRequest request)
        {
            ValidateRequest(request);

            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var module = course.Modules.SingleOrDefault(d => d.Id == moduleId);

            if (module is null)
                throw new ModuleException(ResourceExceptMessages.MODULE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            if (request.Position > course.Modules.Count)
                throw new ModuleException(ResourceExceptMessages.MODULE_POSITION_OUT_RANGE, System.Net.HttpStatusCode.BadRequest);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            _mapper.Map(request, module);

            course.Modules.Remove(module);
            course.Modules.Insert(request.Position - 1, module);

            var response = _mapper.Map<ModuleResponse>(module);
            return response;
        }

        void ValidateRequest(UpdateModuleRequest request)
        {
            var validator = new UpdateModuleValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new ModuleException(errorMessages, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
