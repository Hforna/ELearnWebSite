using AutoMapper;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Modules
{
    public class ChangeModulePosition : IChangeModulePosition
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;

        public ChangeModulePosition(IUnitOfWork uof, IMapper mapper, IUserService userService, SqidsEncoder<long> sqids)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
        }

        public async Task<ModulesResponse> Execute(long courseId, long moduleId, int position)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new ModuleException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var courseTeacherId = course.TeacherId;
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();
            var modules = course.Modules.ToList();

            if (userId != courseTeacherId)
                throw new CourseException(ResourceExceptMessages.COURSE_NOT_OF_USER, System.Net.HttpStatusCode.Unauthorized);

            if (course.Modules.Select(d => d.Id).Contains(moduleId) == false)
                throw new ModuleException(ResourceExceptMessages.MODULE_DOESNT_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var modulesCount = course.Modules.Count;
            var module = course.Modules.SingleOrDefault(d => d.Id == moduleId);

            if (position < 0 || modulesCount < position)
                throw new ModuleException(ResourceExceptMessages.MODULE_POSITION_OUT_RANGE, System.Net.HttpStatusCode.BadRequest);

            if(module.Position != position)
            {
                modules.Remove(module);
                modules.Insert(position - 1, module);
            }

            _uof.moduleWrite.UpdateModules(modules);
            await _uof.Commit();

            var response = _mapper.Map<ModulesResponse>(modules);
            response.CourseId = _sqids.Encode(courseId);

            return response;
        }
    }
}
