using AutoMapper;
using Course.Application.Services.Validators.Module;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface IModuleService
    {
        Task<IList<ModuleResponse>> CreateModuleAsync(CreateModuleRequest request, long courseId);

        Task<ModulesResponse> GetModulesAsync(long courseId);

        Task<ModuleResponse> UpdateModuleAsync(long courseId, long moduleId, UpdateModuleRequest request);

        Task<ModulesResponse> ChangeModulePositionAsync(long courseId, long moduleId, int position);

        Task DeleteModuleAsync(long courseId, long id);
    }

    public class ModuleService : IModuleService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ILinkService _linkService;
        private readonly INewModuleSender _moduleSender;
        private readonly IStorageService _storage;

        public ModuleService(
            IUnitOfWork uof,
            IMapper mapper,
            IUserService userService,
            SqidsEncoder<long> sqids,
            ILinkService linkService,
            INewModuleSender moduleSender,
            IStorageService storage)
        {
            _uof = uof;
            _mapper = mapper;
            _userService = userService;
            _sqids = sqids;
            _linkService = linkService;
            _moduleSender = moduleSender;
            _storage = storage;
        }

        public async Task<IList<ModuleResponse>> CreateModuleAsync(CreateModuleRequest request, long courseId)
        {
            ValidateCreateModule(request);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseByTeacherAndId(userId, courseId);
            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (request.Position is not null && (request.Position < 1 || (request.Position > course.Modules.Count + 1)))
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var module = _mapper.Map<Module>(request);
            module.CourseId = course.Id;

            if (request.Position is not null)
            {
                course.Modules.Insert(module.Position - 1, module);
                var modulesUpdate = course.Modules.Select(m =>
                {
                    m.Position = course.Modules.IndexOf(m) + 1;
                    return m;
                }).ToList();
                _uof.moduleWrite.UpdateModules(modulesUpdate);
            }

            if (request.Position is null)
                module.Position = course.Modules.Count + 1;

            course.ModulesNumber += 1;
            course.Modules.Add(module);

            _uof.courseWrite.UpdateCourse(course);
            _uof.moduleWrite.AddModule(module);
            await _uof.Commit();

            await _moduleSender.SendMessage(module.Id);

            var response = _mapper.Map<IList<ModuleResponse>>(course.Modules);
            return response.Select(m =>
            {
                m.AddLink("lessons", _linkService.GenerateResourceLink("GetLessons", new { id = m.Id }), "GET");
                return m;
            }).ToList();
        }

        public async Task<ModulesResponse> GetModulesAsync(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId, true);
            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = _mapper.Map<ModulesResponse>(course.Modules);
            response.Modules = response.Modules.Select(module =>
            {
                module.AddLink("lessons", _linkService.GenerateResourceLink("GetLessons", new
                {
                    courseId = module.CourseId,
                    moduleId = module.Id
                }), "GET");
                return module;
            }).ToList();

            response.CourseId = _sqids.Encode(course.Id);
            return response;
        }

        public async Task<ModuleResponse> UpdateModuleAsync(long courseId, long moduleId, UpdateModuleRequest request)
        {
            ValidateUpdateModule(request);

            var course = await _uof.courseRead.CourseById(courseId);
            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var module = course.Modules.SingleOrDefault(d => d.Id == moduleId);
            if (module is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (request.Position > course.Modules.Count || request.Position <= 0)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            _mapper.Map(request, module);
            course.Modules.Remove(module);
            course.Modules.Insert(request.Position - 1, module);

            var response = _mapper.Map<ModuleResponse>(module);
            return response;
        }

        public async Task<ModulesResponse> ChangeModulePositionAsync(long courseId, long moduleId, int position)
        {
            var course = await _uof.courseRead.CourseById(courseId);
            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var courseTeacherId = course.TeacherId;

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var modules = course.Modules.ToList();

            if (userId != courseTeacherId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (course.Modules.Select(d => d.Id).Contains(moduleId) == false)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var modulesCount = course.Modules.Count;
            var module = course.Modules.SingleOrDefault(d => d.Id == moduleId);

            if (position < 0 || modulesCount < position)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            if (module.Position != position)
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

        public async Task DeleteModuleAsync(long courseId, long id)
        {
            var module = await _uof.moduleRead.ModuleByCourseAndModuleIds(courseId, id);
            if (module is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (userId != module.Course.TeacherId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            module.Active = false;
            _uof.moduleWrite.UpdateModule(module);
            await _uof.Commit();
        }

        private void ValidateCreateModule(CreateModuleRequest request)
        {
            var validator = new CreateModuleValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);
            }
        }

        private void ValidateUpdateModule(UpdateModuleRequest request)
        {
            var validator = new UpdateModuleValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);
            }
        }
    }
}