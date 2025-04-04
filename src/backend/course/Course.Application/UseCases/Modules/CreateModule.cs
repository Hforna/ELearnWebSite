﻿using AutoMapper;
using Course.Application.Services.Validators.Module;
using Course.Application.UseCases.Repositories.Modules;
using Course.Communication.Requests;
using Course.Communication.Responses;
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

namespace Course.Application.UseCases.Modules
{
    public class CreateModule : ICreateModule
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly ILinkService _linkService;
        private readonly INewModuleSender _moduleSender;

        public CreateModule(IUnitOfWork uof, IMapper mapper, 
            SqidsEncoder<long> sqids, IUserService userService, 
            ILinkService linkService, INewModuleSender moduleSender)
        {
            _uof = uof;
            _mapper = mapper;
            _sqids = sqids;
            _moduleSender = moduleSender;
            _userService = userService;
            _linkService = linkService;
        }


        public async Task<IList<ModuleResponse>> Execute(CreateModuleRequest request, long courseId)
        {
            Validate(request);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseByTeacherAndId(userId, courseId);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            if (request.Position is not null && (request.Position < 1 || (request.Position > course.Modules.Count + 1)))
                throw new CourseException(ResourceExceptMessages.MODULE_POSITION_OUT_RANGE);

            var module = _mapper.Map<Module>(request);
            module.CourseId = course.Id;

            if (request.Position is not null)
            {
                course.Modules.Insert(module.Position - 1, module);
                var modulesUpdate = course.Modules.Select(module =>
                {
                    module.Position = course.Modules.IndexOf(module) + 1;

                    return module;
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

            return response.Select(module =>
            {
                module.AddLink("lessons", _linkService.GenerateResourceLink("GetLessons", new { id = module.Id } ), "GET");

                return module;
            }).ToList();
        }

        void Validate(CreateModuleRequest request)
        {
            var validator = new CreateModuleValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new ModuleException(errorMessages, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
