﻿using AutoMapper;
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
    public class CreateModule : ICreateModules
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
                if (course.Modules.Any(d => d.Position == module.Position))
                {
                    var moduleFilter = course.Modules.Where(d => d.Position >= request.Position).OrderBy(d => d.Position).Select(m =>
                    {
                        m.Position += 1;

                        return m;
                    }).ToList();

                    _uof.moduleWrite.UpdateModules(moduleFilter);
                }
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
    }
}
