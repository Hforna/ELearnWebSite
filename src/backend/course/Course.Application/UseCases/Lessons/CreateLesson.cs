using AutoMapper;
using Course.Application.UseCases.Repositories.Lessons;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class CreateLesson : ICreateLesson
    {
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _uof;

        public CreateLesson(SqidsEncoder<long> sqids, IMapper mapper, IUserService userService, IStorageService storageService, IUnitOfWork uof)
        {
            _sqids = sqids;
            _mapper = mapper;
            _userService = userService;
            _storageService = storageService;
            _uof = uof;
        }

        //public async Task<LessonResponse> Execute(CreateLessonRequest request, long moduleId)
        //{
        //    var module = await _uof.moduleRead.ModuleById(moduleId);
        //
        //    var user = await _userService.GetUserInfos();
        //    var userId = _sqids.Decode(user.id).Single();
        //
        //    if (module.Course.TeacherId != userId)
        //        throw new LessonException(ResourceExceptMessages.COURSE_NOT_OF_USER);
        //
        //
        //}
    }
}
